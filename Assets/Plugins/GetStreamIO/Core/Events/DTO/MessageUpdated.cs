using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core.Events.DTO
{
    public partial class MessageUpdated
    {
        [Newtonsoft.Json.JsonProperty("channel_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Channel_id { get; set; }

        [Newtonsoft.Json.JsonProperty("channel_type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Channel_type { get; set; }

        [Newtonsoft.Json.JsonProperty("cid", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Cid { get; set; }

        [Newtonsoft.Json.JsonProperty("created_at", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset Created_at { get; set; }

        [Newtonsoft.Json.JsonProperty("message", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Message Message { get; set; }

        [Newtonsoft.Json.JsonProperty("team", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Team { get; set; }

        [Newtonsoft.Json.JsonProperty("thread_participants", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<User> Thread_participants { get; }

        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Always)]
        public string Type { get; set; }

        [Newtonsoft.Json.JsonProperty("user", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public User User { get; set; }
    }
}