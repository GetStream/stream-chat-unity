namespace Plugins.GetStreamIO.Core.API
{
    /// <summary>
    ///
    /// </summary>
    public static class ChannelEndpoints
    {
        public static string QueryChannels() => "/channels";

        public static string GetOrCreateAsync(string channelType, string channelId) =>
            $"channels/{channelType}/{channelId}/query";

        public static string GetOrCreateAsync(string channelType) =>
            $"channels/{channelType}/query";
    }
}