using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.API
{
    /// <summary>
    /// Messages API Endpoints
    /// </summary>
    internal static class MessageEndpoints
    {
        public static string SendMessage(string channelType, string channelId)
            => $"/channels/{channelType}/{channelId}/message";

        public static string SendMessage(ChannelState channel) => SendMessage(channel.Channel.Type, channel.Channel.Id);

        public static string UpdateMessage(string messageId) => $"/messages/{messageId}";

        public static string DeleteMessage(string messageId) => $"/messages/{messageId}";

        public static string SendReaction(string messageId) => $"/messages/{messageId}/reaction";

        public static string DeleteReaction(string messageId, string reactionType)
            => $"/messages/{messageId}/reaction/{reactionType}";
    }
}