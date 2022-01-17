using Newtonsoft.Json;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Models.V2;

namespace Plugins.GetStreamIO.Core.Requests.DTO
{
    public class ConnectPayload
    {
        [JsonProperty("user_id")]
        public string UserId;

        [JsonProperty("user_details")]
        public User User;

        [JsonProperty("user_token")]
        public string UserToken;

        [JsonProperty("server_determines_connection_id")]
        public bool ServerDeterminesConnectionId;
    }
}