namespace StreamChat.Core.Configs
{
    /// <summary>
    /// Configuration for <see cref="IStreamChatClient"/>
    /// </summary>
    public interface IStreamClientConfig
    {
        /// <summary>
        /// Configure what type of logs are being emitted.
        /// </summary>
        StreamLogLevel LogLevel { get; set; }
    }
}