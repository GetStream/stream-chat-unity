using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace StreamChat.Libs.Websockets
{
    /// <summary>
    /// Client that communicates with server using websockets protocol
    /// </summary>
    public interface IWebsocketClient : IDisposable
    {
        event Action Connected;
        event Action Disconnected;
        event Action ConnectionFailed;

        bool TryDequeueMessage(out string message);

        /// <summary>
        /// Number of received messages waiting to be dequeued. Receiving runs on a
        /// background timer while consumers dequeue from Unity's main loop, so this reports how
        /// far behind the consumer is. Diagnostic only: the transport never drops messages, and
        /// consumers must not either — a discarded protocol event cannot be recovered, since the
        /// only catch-up mechanism (/sync) is itself limited to roughly 1000 missed events.
        /// </summary>
        int QueuedMessageCount { get; }

        Task ConnectAsync(Uri serverUri, int timeout = 3);

        void Update();

        void Send(string message);

        Task DisconnectAsync(WebSocketCloseStatus closeStatus, string closeMessage);
    }
}