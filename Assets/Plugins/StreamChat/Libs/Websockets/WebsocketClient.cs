﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Libs.Logs;

namespace StreamChat.Libs.Websockets
{
    /// <summary>
    /// Implementation of <see cref="IWebsocketClient"/>
    /// </summary>
    public class WebsocketClient : IWebsocketClient
    {
        public event Action Connected;
        public event Action Disconnected;
        public event Action ConnectionFailed;

        public bool IsConnected => State == WebSocketState.Open;
        public bool IsConnecting => State == WebSocketState.Connecting;

        public WebSocketState State => _internalClient?.State ?? WebSocketState.None;

        /// <param name="isDebugMode">Additional logs will be printed</param>
        public WebsocketClient(ILogs logs, Encoding encoding = default, bool isDebugMode = false)
        {
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));
            _encoding = encoding ?? DefaultEncoding;
            _isDebugMode = isDebugMode;

            var readBuffer = new byte[4 * 1024];
            _bufferSegment = new ArraySegment<byte>(readBuffer);
        }

        public bool TryDequeueMessage(out string message) => _receiveQueue.TryDequeue(out message);

        public async Task ConnectAsync(Uri serverUri)
        {
            if (IsConnected || IsConnecting)
            {
                throw new InvalidOperationException(
                    $"Can't connect during `{State}` state. Please Disconnect() first to cleanup previous connection.");
            }

            _uri = serverUri ?? throw new ArgumentNullException(nameof(serverUri));

            TryDisposeResources(WebSocketCloseStatus.NormalClosure, "Cleaning up resources before connecting");
            _connectionCts = new CancellationTokenSource();

            try
            {
                _internalClient = new ClientWebSocket();
                await _internalClient.ConnectAsync(_uri, _connectionCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException e)
            {
                LogExceptionIfDebugMode(e);
            }
            catch (WebSocketException e)
            {
                LogExceptionIfDebugMode(e);
                OnConnectionFailed();
                return;
            }
            catch (SocketException e)
            {
                LogExceptionIfDebugMode(e);
                OnConnectionFailed();
                return;
            }
            catch (Exception e)
            {
                _logs.Exception(e);
                OnConnectionFailed();
                return;
            }

#pragma warning disable 4014
            Task.Factory.StartNew(SendMessagesLoopAsync, _connectionCts.Token, TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            Task.Factory.StartNew(ReceiveMessagesLoopAsync, _connectionCts.Token, TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
#pragma warning restore 4014

            Connected?.Invoke();
        }

        public void Send(string message)
        {
            var buffer = _encoding.GetBytes(message);
            var messageSegment = new ArraySegment<byte>(buffer);

            _sendQueue.Add(messageSegment);
        }

        public void Update()
        {
            var disconnect = false;
            while (_threadWebsocketExceptionsLog.TryDequeue(out var webSocketException))
            {
                LogExceptionIfDebugMode(webSocketException);

                disconnect = true;
            }

            if (disconnect)
            {
                Disconnect(WebSocketCloseStatus.ProtocolError, "WebSocket thrown an exception");
                return;
            }

            while (_threadExceptionsLog.TryDequeue(out var exception))
            {
                LogExceptionIfDebugMode(exception);
            }
        }

        public void Disconnect(WebSocketCloseStatus closeStatus, string closeMessage)
        {
            LogInfoIfDebugMode("Disconnect");
            TryDisposeResources(closeStatus, closeMessage);

            Disconnected?.Invoke();
        }

        public void Dispose()
        {
            LogInfoIfDebugMode("Dispose");
            Disconnect(WebSocketCloseStatus.NormalClosure, "WebSocket client is disposed");
        }

        private static Encoding DefaultEncoding { get; } = Encoding.UTF8;

        private static readonly WebSocketState[] _clientClosedStates = new[]
            { WebSocketState.Closed, WebSocketState.CloseSent, WebSocketState.CloseReceived, WebSocketState.Aborted };

        private readonly ConcurrentQueue<string> _receiveQueue = new ConcurrentQueue<string>();
        private readonly ConcurrentQueue<Exception> _threadExceptionsLog = new ConcurrentQueue<Exception>();

        private readonly ConcurrentQueue<WebSocketException> _threadWebsocketExceptionsLog =
            new ConcurrentQueue<WebSocketException>();

        private readonly BlockingCollection<ArraySegment<byte>> _sendQueue =
            new BlockingCollection<ArraySegment<byte>>();

        private readonly ArraySegment<byte> _bufferSegment;

        private readonly ILogs _logs;
        private readonly Encoding _encoding;
        private readonly bool _isDebugMode;

        private Uri _uri;
        private ClientWebSocket _internalClient;
        private CancellationTokenSource _connectionCts;

        private async Task SendMessagesLoopAsync()
        {
            while (IsConnected && !_connectionCts.IsCancellationRequested)
            {
                while (_sendQueue.TryTake(out var msg))
                {
                    try
                    {
                        await _internalClient.SendAsync(msg, WebSocketMessageType.Text, true, _connectionCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (WebSocketException webSocketException)
                    {
                        _threadWebsocketExceptionsLog.Enqueue(webSocketException);
                        return;
                    }
                    catch (Exception e)
                    {
                        _threadExceptionsLog.Enqueue(e);
                    }
                }

                await Task.Delay(1);
            }
        }

        private async Task ReceiveMessagesLoopAsync()
        {
            while (IsConnected && !_connectionCts.IsCancellationRequested)
            {
                try
                {
                    var result = await TryReceiveSingleMessageAsync();
                    if (!string.IsNullOrEmpty(result))
                    {
                        _receiveQueue.Enqueue(result);
                        continue;
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (WebSocketException webSocketException)
                {
                    _threadWebsocketExceptionsLog.Enqueue(webSocketException);
                    return;
                }
                catch (Exception e)
                {
                    _threadExceptionsLog.Enqueue(e);
                }

                await Task.Delay(1);
            }
        }

        private void TryDisposeResources(WebSocketCloseStatus closeStatus, string closeMessage)
        {
            try
            {
                _connectionCts?.Cancel();
                _connectionCts?.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }

            if (_internalClient != null)
            {
                if (!_clientClosedStates.Contains(_internalClient.State))
                {
                    _internalClient.CloseOutputAsync(closeStatus, closeMessage, CancellationToken.None);
                }

                _internalClient.Dispose();
                _internalClient = null;
            }
        }

        private void OnConnectionFailed() => ConnectionFailed?.Invoke();

        private void OnReceivedCloseMessage()
            => Disconnect(WebSocketCloseStatus.InternalServerError, "Server closed the connection");

        private async Task<string> TryReceiveSingleMessageAsync()
        {
            using (var ms = new MemoryStream())
            {
                if (_internalClient.State != WebSocketState.Open)
                {
                    throw new InvalidOperationException(
                        "Tried to receive WebSocket message but connection is not Open");
                }

                WebSocketReceiveResult chunkResult;
                do
                {
                    chunkResult = await _internalClient.ReceiveAsync(_bufferSegment, _connectionCts.Token);

                    if (chunkResult.MessageType == WebSocketMessageType.Close)
                    {
                        OnReceivedCloseMessage();
                        return "";
                    }

                    ms.Write(_bufferSegment.Array, _bufferSegment.Offset, chunkResult.Count);
                } while (!chunkResult.EndOfMessage && !_connectionCts.IsCancellationRequested);

                _connectionCts.Token.ThrowIfCancellationRequested();

                //reset position before reading from stream
                ms.Seek(0, SeekOrigin.Begin);

                if (chunkResult.MessageType == WebSocketMessageType.Text)
                {
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }

                if (chunkResult.MessageType == WebSocketMessageType.Binary)
                {
                    throw new Exception("Unhandled WebSocket message type: " + WebSocketMessageType.Binary);
                }

                return "";
            }
        }

        private void LogExceptionIfDebugMode(Exception exception)
        {
            if (_isDebugMode)
            {
                _logs.Exception(exception);
            }
        }

        private void LogInfoIfDebugMode(string info)
        {
            if (_isDebugMode)
            {
                _logs.Info(info);
            }
        }
    }
}