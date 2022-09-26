using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Utils;

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

        public NativeWebSocketWrapper(ILogs logs)
        {
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));
        }

        public void Dispose() => Disconnect();

        public bool TryDequeueMessage(out string message)
        {
            message = _messages.Count > 0 ? _messages.Dequeue() : null;
            return message != null;
        }

        public async Task ConnectAsync(Uri serverUri)
        {
            if (_webSocket != null)
            {
                await DisconnectAsync();
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

        public void Disconnect()
        {
            DisconnectAsync().LogIfFailed(_logs);
            Disconnected?.Invoke();
        }

        public void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _webSocket.DispatchMessageQueue();
#endif
        }

        public void Send(string message)
        {
            try
            {
                _webSocket.SendText(message).ContinueWith(_ => { _logs.Exception(_.Exception); },
                    TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception e)
            {
                _logs.Exception(e);
            }
        }

        private readonly ILogs _logs;
        private readonly Queue<string> _messages = new Queue<string>();

        private WebSocket _webSocket;

        private Task DisconnectAsync()
        {
            if (_webSocket.State == WebSocketState.Closing || _webSocket.State == WebSocketState.Closed)
            {
                return Task.CompletedTask;
            }

            UnsubscribeFromEvents();
            return _webSocket.Close();
        }

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
    }
}