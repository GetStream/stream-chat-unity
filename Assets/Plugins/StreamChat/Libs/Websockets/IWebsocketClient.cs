using System;
using System.Threading.Tasks;

namespace StreamChat.Libs.Websockets
{
    /// <summary>
    /// Client that communicates with server using websockets protocol
    /// </summary>
    public interface IWebsocketClient : IDisposable
    {
        event Action Connected;
        event Action ConnectionFailed;

        bool IsRunning { get; }

        bool TryDequeueMessage(out string message);

        Task ConnectAsync(Uri serverUri);

        Task Send(string message);
    }
}