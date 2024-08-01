namespace StreamChat.Core.InternalDTO.Extra
{
    internal class UnreadCountsChannelInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("channel_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ChannelId { get; set; }

        [Newtonsoft.Json.JsonProperty("last_read", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset LastRead { get; set; }

        [Newtonsoft.Json.JsonProperty("unread_count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int UnreadCount { get; set; }
    }
}