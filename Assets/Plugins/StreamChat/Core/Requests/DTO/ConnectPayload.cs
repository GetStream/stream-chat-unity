using Newtonsoft.Json;
using StreamChat.Core.Models;

namespace StreamChat.Core.Requests.DTO
{
    internal class ConnectPayload
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