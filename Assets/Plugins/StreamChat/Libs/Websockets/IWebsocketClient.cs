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

        Task ConnectAsync(Uri serverUri);

        void Update();

        void Send(string message);

        Task DisconnectAsync(WebSocketCloseStatus closeStatus, string closeMessage);
    }
}