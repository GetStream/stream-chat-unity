using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core.API
{
    /// <summary>
    ///
    /// </summary>
    public static class MessageEndpoints
    {
        public static string SendMessage(string channelType, string channelId) => $"/channels/{channelType}/{channelId}/message";
        public static string SendMessage(ChannelState channel) => SendMessage(channel.Channel.Type, channel.Channel.Id);
    }
}