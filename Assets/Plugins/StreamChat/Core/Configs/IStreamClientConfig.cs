using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Configs
{
    /// <summary>
    /// Configuration for <see cref="IStreamChatClient"/>
    /// </summary>
    public interface IStreamClientConfig
    {
        /// <summary>
        /// What type of logs are being emitted.
        /// </summary>
        StreamLogLevel LogLevel { get; set; }
    }
}