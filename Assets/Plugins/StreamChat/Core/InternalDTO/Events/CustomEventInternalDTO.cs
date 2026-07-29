using System;
using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.InternalDTO.Events
{
    // Hand-written (not OpenAPI-generated). As of today the OpenAPI spec CustomEvent schema is invalid
    // and missing key fields required for WS dispatch (cid, user, channel_type, channel_id, parent_id).
    internal sealed class CustomEventInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("type")]
        public string Type { get; set; }

        [Newtonsoft.Json.JsonProperty("cid")]
        public string Cid { get; set; }

        [Newtonsoft.Json.JsonProperty("channel_type")]
        public string ChannelType { get; set; }

        [Newtonsoft.Json.JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [Newtonsoft.Json.JsonProperty("parent_id")]
        public string ParentId { get; set; }

        [Newtonsoft.Json.JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Newtonsoft.Json.JsonProperty("user")]
        public UserObjectInternalDTO User { get; set; }

        [Newtonsoft.Json.JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}
