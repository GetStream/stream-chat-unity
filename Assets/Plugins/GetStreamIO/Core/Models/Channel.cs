using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Models
{
    /// <summary>
    /// Channel model
    /// </summary>
    public class Channel
    {
        [JsonProperty("channel")]
        public ChannelDetails Details;

        [JsonProperty("messages")]
        public List<Message> Messages = new List<Message>();
    }
}