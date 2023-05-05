namespace StreamChat.Core.Configs
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public class StreamClientConfig : IStreamClientConfig
    {
        public static IStreamClientConfig Default { get; set; } = new StreamClientConfig();

        public StreamLogLevel LogLevel { get; set; } = StreamLogLevel.FailureOnly;
    }
}