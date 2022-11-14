﻿namespace StreamChat.Core
{
    /// <summary>
    /// Strategy for <see cref="IStreamChatClient"/> when connection is lost
    /// </summary>
    public enum ReconnectStrategy
    {
        /// <summary>
        /// Reconnect attempts will occur at exponentially increasing intervals
        /// </summary>
        Exponential,

        /// <summary>
        /// Reconnect attempts will occur at constant interval
        /// </summary>
        Constant,

        /// <summary>
        /// The Stream Chat Client will never attempt to reconnect. You need to call the <see cref="IStreamChatClient.Connect"/> on your own
        /// </summary>
        Never,
    }
}