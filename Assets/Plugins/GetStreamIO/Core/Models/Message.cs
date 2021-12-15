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

        [JsonProperty("text")]
        public string Text;

        [JsonProperty("type")]
        public MessageType Type;

        [JsonProperty("user")]
        public User User;
    }
}