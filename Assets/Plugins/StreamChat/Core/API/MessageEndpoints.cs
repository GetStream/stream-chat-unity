using StreamChat.Core.Models;

namespace StreamChat.Core.API
{
    /// <summary>
    ///
    /// </summary>
    public static class MessageEndpoints
    {
        public static string SendMessage(string channelType, string channelId) => $"/channels/{channelType}/{channelId}/message";

        public static string SendMessage(ChannelState channel) => SendMessage(channel.Channel.Type, channel.Channel.Id);

        public static string UpdateMessage(string messageId) => $"/messages/{messageId}";
        public static string DeleteMessage(string messageId) => $"/messages/{messageId}";
    }
}