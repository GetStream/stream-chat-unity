using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Models
{
    public class Member
    {
        [JsonProperty("user_id")]
        public string UserId;

        [JsonProperty("user")]
        public User User;
    }
}