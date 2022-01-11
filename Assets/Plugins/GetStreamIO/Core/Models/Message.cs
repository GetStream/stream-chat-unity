using GetStreamIO.Core.DTO.Models;
using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Models
{
    /// <summary>
    /// Message model
    /// </summary>
    public class Message
    {
        [JsonProperty("id")]
        public string Id;

        [Newtonsoft.Json.JsonProperty("cid", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Cid { get; set; }

        [JsonProperty("text")]
        public string Text;

        [JsonProperty("type")]
        public MessageType Type;

        [JsonProperty("user")]
        public User User;
    }
}