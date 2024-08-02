namespace StreamChat.Core.InternalDTO.Extra
{
    internal class WrappedUnreadCountsResponseInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("channel_type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<UnreadCountsChannelTypeInternalDTO> ChannelType { get; set; } = new System.Collections.Generic.List<UnreadCountsChannelTypeInternalDTO>();

        [Newtonsoft.Json.JsonProperty("channels", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<UnreadCountsChannelInternalDTO> Channels { get; set; } = new System.Collections.Generic.List<UnreadCountsChannelInternalDTO>();

        /// <summary>
        /// Duration of the request in milliseconds
        /// </summary>
        [Newtonsoft.Json.JsonProperty("duration", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Duration { get; set; }

        [Newtonsoft.Json.JsonProperty("threads", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<UnreadCountsThreadInternalDTO> Threads { get; set; } = new System.Collections.Generic.List<UnreadCountsThreadInternalDTO>();

        [Newtonsoft.Json.JsonProperty("total_unread_count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int TotalUnreadCount { get; set; }

        [Newtonsoft.Json.JsonProperty("total_unread_threads_count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int TotalUnreadThreadsCount { get; set; }
    }
}