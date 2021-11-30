using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Models
{
    /// <summary>
    /// Channel model
    /// </summary>
    public class ChannelDetails
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("member_count")]
        public int MemberCount;

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt;

        [JsonProperty("created_by")]
        public User CreatedBy;

        public List<Message> Messages = new List<Message>();
    }
}