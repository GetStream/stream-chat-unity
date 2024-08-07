namespace StreamChat.Core.InternalDTO.Extra
{
    internal class UnreadCountsThreadInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("last_read", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset LastRead { get; set; }

        [Newtonsoft.Json.JsonProperty("last_read_message_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string LastReadMessageId { get; set; }

        [Newtonsoft.Json.JsonProperty("parent_message_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ParentMessageId { get; set; }

        [Newtonsoft.Json.JsonProperty("unread_count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int UnreadCount { get; set; }
    }
}