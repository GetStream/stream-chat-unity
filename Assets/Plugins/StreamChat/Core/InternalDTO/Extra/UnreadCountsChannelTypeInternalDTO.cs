namespace StreamChat.Core.InternalDTO.Extra
{
    internal class UnreadCountsChannelTypeInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("channel_count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int ChannelCount { get; set; }

        [Newtonsoft.Json.JsonProperty("channel_type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ChannelType { get; set; }

        [Newtonsoft.Json.JsonProperty("unread_count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int UnreadCount { get; set; }
    }
}