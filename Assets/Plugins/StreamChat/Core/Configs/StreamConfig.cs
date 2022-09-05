namespace StreamChat.Core.Configs
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public class StreamConfig : IStreamConfig
    {
        public static IStreamConfig Default { get; set; } = new StreamConfig();

        public StreamLogLevel LogLevel { get; set; } = StreamLogLevel.All;
    }
}