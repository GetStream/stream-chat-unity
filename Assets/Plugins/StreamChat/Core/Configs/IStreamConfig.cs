namespace StreamChat.Core.Configs
{
    /// <summary>
    /// Configuration for <see cref="IStreamChatClient"/>
    /// </summary>
    public interface IStreamConfig
    {
        /// <summary>
        /// Configure what type of logs are being emitted.
        /// </summary>
        StreamLogLevel LogLevel { get; set; }
    }
}