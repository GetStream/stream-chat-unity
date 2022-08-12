using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
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

        public const bool IsDebugMode = false;

        public bool IsConnected => State == WebSocketState.Open;
        public bool IsConnecting => State == WebSocketState.Connecting;

        public WebSocketState State => _internalClient?.State ?? WebSocketState.None;

        public WebsocketClient(ILogs logs, Encoding encoding = default)
        {
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));
            _encoding = encoding ?? DefaultEncoding;

            _logs.Prefix = "StreamChat Internal Websocket Client";

            var readBuffer = new byte[4 * 1024];
            _bufferSegment = new ArraySegment<byte>(readBuffer);
        }

        public bool TryDequeueMessage(out string message)
            => _receiveQueue.TryDequeue(out message);

        public async Task ConnectAsync(Uri serverUri)
        {
            if (IsConnected || IsConnecting)
            {
                throw new InvalidOperationException(
                    $"Can't connect during `{State}` state. Please Disconnect() first to cleanup previous connection.");
            }

            _uri = serverUri ?? throw new ArgumentNullException(nameof(serverUri));

            _connectionCts?.Dispose();
            _connectionCts = new CancellationTokenSource();

            try
            {
                _internalClient = new ClientWebSocket();
                await _internalClient.ConnectAsync(_uri, _connectionCts.Token).ConfigureAwait(false);
            }
            catch (WebSocketException e)
            {
                if (IsDebugMode)
                {
                    _logs.Exception(e);
                }
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
                if (IsDebugMode)
                {
                    _logs.Exception(webSocketException);
                }

                disconnect = true;
            }

            if (disconnect)
            {
                Disconnect();
                return;
            }

            while (_threadExceptionsLog.TryDequeue(out var exception))
            {
                _logs.Exception(exception);
            }
        }

        public void Disconnect()
        {
            _logs.Info("Disconnect");
            _connectionCts?.Dispose();

            if (_internalClient != null && !_clientClosedStates.Contains(_internalClient.State))
            {
                _internalClient?.CloseOutputAsync(WebSocketCloseStatus.Empty, statusDescription: null, CancellationToken.None);
                _internalClient?.Dispose();
                _internalClient = null;
            }

            Disconnected?.Invoke();
        }

        public void Dispose()
            => Disconnect();

        private static Encoding DefaultEncoding { get; } = Encoding.UTF8;

        private readonly WebSocketState[] _clientClosedStates = new[]
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

        private Uri _uri;
        private ClientWebSocket _internalClient;
        private CancellationTokenSource _connectionCts;

        private async Task SendMessagesLoopAsync()
        {
            while (IsConnected && !_connectionCts.IsCancellationRequested)
            {
                while (!_sendQueue.IsCompleted)
                {
                    var msg = _sendQueue.Take();

                    try
                    {
                        await _internalClient.SendAsync(msg, WebSocketMessageType.Text, true, CancellationToken.None);
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
                catch (WebSocketException webSocketException)
                {
                    _threadWebsocketExceptionsLog.Enqueue(webSocketException);
                    return;
                }
                catch (Exception e)
                {
                    _threadExceptionsLog.Enqueue(e);
                }

                Task.Delay(50).Wait();
            }
        }

        private void OnConnectionFailed()
            => ConnectionFailed?.Invoke();

        private void OnReceivedCloseMessage() => Disconnect();

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
    }
}