namespace StreamChat.Core
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
        /// The client will never attempt to reconnect. The application will have manage calling Connect() when connection is lost
        /// </summary>
        Never,
    }
}