using System.Collections.Generic;
using Newtonsoft.Json;
using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core.Requests.DTO
{
    public class SendMessage
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("text")]
        public string Text;

        [JsonProperty("attachments")]
        public List<string> Attachments = new List<string>();

        [JsonProperty("mentioned_users")]
        public List<User> MentionedUsers = new List<User>();
    }
}