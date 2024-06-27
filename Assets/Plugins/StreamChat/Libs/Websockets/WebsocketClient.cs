using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Libs.Logs;
#if STREAM_DEBUG_ENABLED
using System.Diagnostics;
#endif

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
        }

        public bool TryDequeueMessage(out string message) => _receiveQueue.TryDequeue(out message);

        public async Task ConnectAsync(Uri serverUri, int timeout = 3)
        {
            if (IsConnected || IsConnecting)
            {
                throw new InvalidOperationException(
                    $"Can't connect during `{State}` state. Please Disconnect() first to cleanup previous connection.");
            }

            _uri = serverUri ?? throw new ArgumentNullException(nameof(serverUri));

            try
            {
                await TryCloseAndDisposeAsync(WebSocketCloseStatus.NormalClosure,
                    "Clean up resources before connecting");
                _connectionCts = new CancellationTokenSource();

                _internalClient = new ClientWebSocket();

#if STREAM_DEBUG_ENABLED
                var ws = new Stopwatch();
                ws.Start();
                _logs.Warning($"Internal WS ConnectAsync CALL");
#endif

                var connectTask = _internalClient.ConnectAsync(_uri, _connectionCts.Token);
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeout));

                // We handle timeout this way because ConnectAsync was hanging after multiple attempts on Unity 2022.3.29 & Android 14 and cancellation via passed token didn't work
                var finishedTask = await Task.WhenAny(connectTask, timeoutTask);

                if (finishedTask == timeoutTask)
                {
#if STREAM_DEBUG_ENABLED
                    _logs.Warning("Internal WS Connection attempt timed out.");
#endif
                    throw new TimeoutException($"Connection attempt timed out after {timeout} seconds.");
                }

                if (_connectionCts == null || _connectionCts.Token.IsCancellationRequested)
                {
#if STREAM_DEBUG_ENABLED
                    _logs.Warning("Internal WS Connection attempt cancelled.");
#endif
                    throw new OperationCanceledException();
                }

                await connectTask;

#if STREAM_DEBUG_ENABLED
                ws.Stop();
                _logs.Warning($"Internal WS ConnectAsync COMPLETED in {ws.ElapsedMilliseconds} ms.");
#endif
            }
            catch (Exception e)
            {
                await HandleConnectionFailedAsync(e);
                return;
            }

            _backgroundSendTimer = new Timer(SendMessagesCallback, null, 0, UpdatePeriod);
            _backgroundReceiveTimer = new Timer(ReceiveMessagesCallback, null, UpdatePeriodOffset, UpdatePeriod);

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
#if STREAM_DEBUG_ENABLED

            if (_internalClient != null && _internalClient.State != _lastState)
            {
                _logs.Warning($"Internal WS state -> changed from {_lastState} to " + _internalClient.State);
                _lastState = _internalClient.State;
            }
#endif

            var disconnect = false;
            while (_threadWebsocketExceptionsLog.TryDequeue(out var webSocketException))
            {
                LogExceptionIfDebugMode(webSocketException);

                disconnect = true;
            }

            if (disconnect)
            {
                DisconnectAsync(WebSocketCloseStatus.ProtocolError, "WebSocket thrown an exception")
                    .ContinueWith(_ => LogExceptionIfDebugMode(_.Exception), TaskContinuationOptions.OnlyOnFaulted);
                return;
            }

            while (_threadExceptionsLog.TryDequeue(out var exception))
            {
                LogExceptionIfDebugMode(exception);
            }
        }

        public async Task DisconnectAsync(WebSocketCloseStatus closeStatus, string closeMessage)
        {
            LogInfoIfDebugMode("Disconnect");
            await TryCloseAndDisposeAsync(closeStatus, closeMessage);

            Disconnected?.Invoke();
        }

        public void Dispose()
        {
            LogInfoIfDebugMode("Dispose " + Thread.CurrentThread.ManagedThreadId);

            if (_internalClient != null && !_clientClosedStates.Contains(_internalClient.State))
            {
                DisconnectAsync(WebSocketCloseStatus.NormalClosure, "WebSocket client is disposed")
                    .ContinueWith(t => LogExceptionIfDebugMode(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        private const int UpdatesPerSecond = 20;
        private const int UpdatePeriod = 1000 / UpdatesPerSecond;
        private const int UpdatePeriodOffset = UpdatePeriod / 2;

        private static Encoding DefaultEncoding { get; } = Encoding.UTF8;

        private static readonly WebSocketState[] _clientClosedStates = new[]
            { WebSocketState.Closed, WebSocketState.CloseSent, WebSocketState.CloseReceived, WebSocketState.Aborted };

        private readonly ConcurrentQueue<string> _receiveQueue = new ConcurrentQueue<string>();
        private readonly ConcurrentQueue<Exception> _threadExceptionsLog = new ConcurrentQueue<Exception>();

        private readonly ConcurrentQueue<WebSocketException> _threadWebsocketExceptionsLog =
            new ConcurrentQueue<WebSocketException>();

        private readonly BlockingCollection<ArraySegment<byte>> _sendQueue =
            new BlockingCollection<ArraySegment<byte>>();

        private readonly ArraySegment<byte> _bufferSegment = new ArraySegment<byte>(new byte[4 * 1024]);

        private readonly ILogs _logs;
        private readonly Encoding _encoding;
        private readonly bool _isDebugMode;

        private readonly SemaphoreSlim _backgroundSendSemaphore = new SemaphoreSlim(1);
        private readonly SemaphoreSlim _backgroundReceiveSemaphore = new SemaphoreSlim(1);

        private Timer _backgroundSendTimer;
        private Timer _backgroundReceiveTimer;

        private Uri _uri;
        private ClientWebSocket _internalClient;
        private CancellationTokenSource _connectionCts;

        private WebSocketState _lastState;

        private async void SendMessagesCallback(object state)
        {
            if (!IsConnected || _connectionCts == null || _connectionCts.IsCancellationRequested)
            {
                return;
            }

            if (!_backgroundSendSemaphore.Wait(0))
            {
                return;
            }

            try
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
            }
            finally
            {
                _backgroundSendSemaphore.Release();
            }
        }

        // Runs on a background thread
        private async void ReceiveMessagesCallback(object state)
        {
            if (!IsConnected || _connectionCts == null || _connectionCts.IsCancellationRequested)
            {
                return;
            }

            if (!_backgroundReceiveSemaphore.Wait(0))
            {
                return;
            }

            try
            {
                var result = await TryReceiveSingleMessageAsync();
                if (!string.IsNullOrEmpty(result))
                {
                    _receiveQueue.Enqueue(result);
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
            finally
            {
                _backgroundReceiveSemaphore.Release();
            }
        }

        private async Task TryCloseAndDisposeAsync(WebSocketCloseStatus closeStatus, string closeMessage)
        {
            try
            {
#if UNITY_2021_2_OR_NEWER
                if (_backgroundReceiveTimer != null)
                {
                    await _backgroundReceiveTimer.DisposeAsync();
                    _backgroundReceiveTimer = null;
                }
#else
                if (_backgroundReceiveTimer != null)
                {
                    _backgroundReceiveTimer.Dispose();
                    _backgroundReceiveTimer = null;
                }
#endif
            }
            catch (Exception e)
            {
                LogExceptionIfDebugMode(e);
            }

            try
            {
#if UNITY_2021_2_OR_NEWER
                if (_backgroundSendTimer != null)
                {
                    await _backgroundSendTimer.DisposeAsync();
                    _backgroundSendTimer = null;
                }
#else
                if (_backgroundSendTimer != null)
                {
                    _backgroundSendTimer.Dispose();
                    _backgroundSendTimer = null;
                }
#endif

            }
            catch (Exception e)
            {
                LogExceptionIfDebugMode(e);
            }

            try
            {
                if (_connectionCts != null)
                {
                    if (!_connectionCts.IsCancellationRequested)
                    {
                        _connectionCts.Cancel();
                    }

                    _connectionCts.Dispose();
                    _connectionCts = null;
                }
            }
            catch (Exception e)
            {
                LogExceptionIfDebugMode(e);
            }

            if (_internalClient == null)
            {
                return;
            }

            try
            {
                if (_internalClient.State == WebSocketState.Open)
                {
#if STREAM_DEBUG_ENABLED
                    _logs.Warning("Internal WS - Disposing; Is open -> CloseOutputAsync");
#endif
                    await _internalClient.CloseOutputAsync(closeStatus, closeMessage, CancellationToken.None);
                }

                if (_internalClient.State == WebSocketState.Connecting)
                {
#if STREAM_DEBUG_ENABLED
                    _logs.Warning("Internal WS - Disposing; Is Connecting -> Abort");
#endif
                    _internalClient.Abort();
                }
            }
            catch (Exception e)
            {
                LogExceptionIfDebugMode(e);
            }
            finally
            {
#if STREAM_DEBUG_ENABLED
                _logs.Warning("Internal WS - Dispose in state: " + _internalClient.State);
#endif
                _internalClient.Dispose();
                _internalClient = null;
            }
        }

        private async Task HandleConnectionFailedAsync(Exception exception)
        {
#if STREAM_DEBUG_ENABLED
            _logs.Warning("Internal WS - Connection Failed - trigger ConnectionFailed event");
#endif

            try
            {
                await TryCloseAndDisposeAsync(WebSocketCloseStatus.ProtocolError,
                    "Closing due to exception thrown during connection attempt: " + exception.Message);
            }
            catch (Exception e)
            {
                _logs.Exception(e);
            }
            
            var isHandledExceptionType = exception is OperationCanceledException || exception is WebSocketException || exception is SocketException;
            if (isHandledExceptionType)
            {
                LogExceptionIfDebugMode(exception);
            }
            else
            {
                _logs.Exception(exception);
            }
           
            ConnectionFailed?.Invoke();
        }

        // Called from a background thread
        private void OnReceivedCloseMessage()
            => DisconnectAsync(WebSocketCloseStatus.InternalServerError, "Server closed the connection")
                .ContinueWith(t => LogThreadExceptionIfDebugMode(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

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

                return string.Empty;
            }
        }

        private void LogExceptionIfDebugMode(Exception exception)
        {
            if (_isDebugMode)
            {
                _logs.Exception(exception);
            }
        }

        private void LogThreadExceptionIfDebugMode(Exception exception)
        {
            if (_isDebugMode)
            {
                _threadExceptionsLog.Enqueue(exception);
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