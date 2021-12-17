using Newtonsoft.Json;
using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core.Events.DTO
{
    /// <summary>
    /// New message vent
    /// </summary>
    public class MessageDeletedEvent : BaseEvent
    {
        [JsonProperty("channel_id")]
        public string ChannelId;

        [JsonProperty("message")]
        public Message Message;
    }
}