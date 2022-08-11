using System;
using StreamChat.Core.API;
using StreamChat.Core.Auth;
using StreamChat.Core.Models;

namespace StreamChat.Core
{
    /// <summary>
    /// Stream Chat Client
    /// </summary>
    public interface IStreamChatClient : IAuthProvider, IConnectionProvider, IStreamRealtimeEventsProvider, IDisposable
    {
        /// <summary>
        /// Client established WebSockets connection and is ready to send and receive data
        /// </summary>
        event ConnectionHandler Connected;

        /// <summary>
        /// Raised when connection state changes. Returns respectively previous and the current connection state
        /// </summary>
        event Action<ConnectionState, ConnectionState> ConnectionStateChanged;

        ConnectionState ConnectionState { get; }
        ReconnectStrategy ReconnectStrategy { get; set; }

        IChannelApi ChannelApi { get; }
        IMessageApi MessageApi { get; }
        IModerationApi ModerationApi { get; }
        IUserApi UserApi { get; }
        OwnUser LocalUser { get; }

        void Update(float deltaTime);

        /// <summary>
        /// Initiate WebSocket connection with Stream Server.
        /// Use <see cref="IConnectionProvider.Connected"/> to be notified when connection is established
        /// </summary>
        void Connect();

        bool IsLocalUser(User user);

        bool IsLocalUser(ChannelMember channelMember);
    }
}