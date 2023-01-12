using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.API
{
    /// <summary>
    /// <see cref="Channel"/> endpoints
    /// </summary>
    internal static class ChannelEndpoints
    {
        public static string QueryChannels() => "/channels";

        public static string GetOrCreate(string type, string id) => $"channels/{type}/{id}/query";

        public static string GetOrCreate(string type) => $"channels/{type}/query";

        public static string Update(string type, string id) => $"channels/{type}/{id}";

        public static string UpdatePartial(string type, string id) => $"channels/{type}/{id}";

        public static string DeleteChannel(string type, string id) => $"/channels/{type}/{id}";

        public static string DeleteChannels() => "channels/delete";

        public static string TruncateChannel(string type, string id) => $"/channels/{type}/{id}/truncate";

        public static string MuteChannel() => $"/moderation/mute/channel";

        public static string UnmuteChannel() => $"/moderation/unmute/channel";

        public static string ShowChannel(string type, string id) => $"/channels/{type}/{id}/show";

        public static string HideChannel(string type, string id) => $"/channels/{type}/{id}/hide";
    }
}