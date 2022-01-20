using System;

namespace StreamChat.Core
{
    /// <summary>
    /// Provides connection id
    /// </summary>
    public interface IConnectionProvider
    {
        Uri ServerUri { get; }
        string ConnectionId { get; }
    }
}