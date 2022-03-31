using System;

namespace StreamChat.Core
{
    /// <summary>
    /// Provides connection id
    /// </summary>
    public interface IConnectionProvider
    {
        event Action Connected;
        event Action<ConnectionState, ConnectionState> ConnectionStateChanged;
        Uri ServerUri { get; }
        string ConnectionId { get; }
    }
}