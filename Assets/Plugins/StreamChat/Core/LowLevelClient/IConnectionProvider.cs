using System;

namespace StreamChat.Core.LowLevelClient
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