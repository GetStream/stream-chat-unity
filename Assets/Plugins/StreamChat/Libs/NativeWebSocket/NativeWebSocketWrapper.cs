using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Utils;
using WebSocket = NativeWebSocket.WebSocket;
using WebSocketState = NativeWebSocket.WebSocketState;

namespace StreamChat.Libs.Websockets
{
    /// <summary>
    /// IWebsocketClient wrapper for https://github.com/endel/NativeWebSocket
    /// </summary>
    public class NativeWebSocketWrapper : IWebsocketClient
    {
        public event Action Connected;
        public event Action Disconnected;
        public event Action ConnectionFailed;

        public NativeWebSocketWrapper(ILogs logs, bool isDebugMode)
        {
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));
            _isDebugMode = isDebugMode;
        }

        public void Dispose() => DisconnectAsync().LogIfFailed(_logs);

        public bool TryDequeueMessage(out string message)
        {
            message = _messages.Count > 0 ? _messages.Dequeue() : null;
            return message != null;
        }

        public async Task ConnectAsync(Uri serverUri)
        {
            if (_webSocket != null)
            {
                if(_webSocket.State == WebSocketState.Open)
                {
                    LogInfoIfDebugMode("Internal WS was open, try to disconnect");
                    await DisconnectAsync();
                }

                if (_webSocket.State == WebSocketState.Connecting)
                {
                    LogWarningIfDebugMode("Internal WS is already connecting");
                }
            }

            _webSocket = new WebSocket(serverUri.ToString());

            SubscribeToEvents();

            try
            {
                await _webSocket.Connect();
            }
            catch (Exception)
            {
                ConnectionFailed?.Invoke();
                throw;
            }
        }

        public Task DisconnectAsync(WebSocketCloseStatus closeStatus, string closeMessage) => DisconnectAsync();

        public async Task DisconnectAsync()
        {
            if (_webSocket == null)
            {
                return;
            }

            if (_webSocket.State == WebSocketState.Closing || _webSocket.State == WebSocketState.Closed)
            {
                Disconnected?.Invoke();
                return;
            }

            UnsubscribeFromEvents();
            await _webSocket.Close();
            Disconnected?.Invoke();
        }

        public void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _webSocket?.DispatchMessageQueue();
#endif
        }

        public void Send(string message)
        {
            if (_webSocket == null)
            {
                _logs.Error($"Tried to send a message but {nameof(_webSocket)} is null.");
                return;
            }

            if (_webSocket.State != WebSocketState.Open)
            {
                _logs.Error($"Tried to send a message but {nameof(_webSocket.State)} is: {_webSocket.State}. Expected: {nameof(WebSocketState.Open)}");
                return;
            }
            
            try
            {
                _webSocket.SendText(message).ContinueWith(_ =>
                    {
                        if (_.IsFaulted)
                        {
                            _logs.Exception(_.Exception);
                        }
                    },
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception e)
            {
                _logs.Exception(e);
            }
        }

        private readonly ILogs _logs;
        private readonly Queue<string> _messages = new Queue<string>();

        private WebSocket _webSocket;
        private readonly bool _isDebugMode;

        private void SubscribeToEvents()
        {
            _webSocket.OnOpen += OnWebSocketOpen;
            _webSocket.OnClose += OnWebSocketClose;
            _webSocket.OnMessage += OnWebSocketMessage;
            _webSocket.OnError += OnWebSocketError;
        }

        private void UnsubscribeFromEvents()
        {
            _webSocket.OnOpen -= OnWebSocketOpen;
            _webSocket.OnClose -= OnWebSocketClose;
            _webSocket.OnMessage -= OnWebSocketMessage;
            _webSocket.OnError -= OnWebSocketError;
        }

        private void OnWebSocketOpen() => Connected?.Invoke();

        private void OnWebSocketClose(WebSocketCloseCode closeCode) => Disconnected?.Invoke();

        private void OnWebSocketMessage(byte[] data) => _messages.Enqueue(Encoding.UTF8.GetString(data));

        private void OnWebSocketError(string errorMsg) => _logs.Error(errorMsg);
        
        private void LogInfoIfDebugMode(string info)
        {
            if (_isDebugMode)
            {
                _logs.Info(info);
            }
        }
        
        private void LogWarningIfDebugMode(string info)
        {
            if (_isDebugMode)
            {
                _logs.Info(info);
            }
        }
    }
}