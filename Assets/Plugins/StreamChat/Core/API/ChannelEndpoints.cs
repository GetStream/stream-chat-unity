using StreamChat.Core.Models;

namespace StreamChat.Core.API
{
    /// <summary>
    /// <see cref="Channel"/> endpoints
    /// </summary>
    public static class ChannelEndpoints
    {
        public static string QueryChannels() => "/channels";

        public static string GetOrCreateAsync(string type, string id) => $"channels/{type}/{id}/query";

        public static string GetOrCreateAsync(string type) => $"channels/{type}/query";

        public static string Update(string type, string id) => $"channels/{type}/{id}";

        public static string UpdatePartial(string type, string id) => $"channels/{type}/{id}";

        public static string DeleteChannel(string type, string id) => $"/channels/{type}/{id}";

        public static string DeleteChannels() => "channels/delete";

        public static string TruncateChannel(string type, string id) => $"/channels/{type}/{id}/truncate";

        public static string MuteChannel() => $"/moderation/mute/channel";
        public static string UnmuteChannel() => $"/moderation/unmute/channel";
    }
}