using System.Collections.Generic;
using Newtonsoft.Json;
using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core.Requests.DTO
{
    public class ChannelsResponse
    {
        [JsonProperty("channels")]
        public List<Channel> Channels = new List<Channel>();
    }
}