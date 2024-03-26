using Newtonsoft.Json;

namespace StreamChat.Core.LowLevelClient.Requests.DTO
{
    internal class ConnectPayload
    {
        [JsonProperty("user_id")]
        public string UserId;

        [JsonProperty("user_details")]
        public UserObjectRequest User;

        [JsonProperty("user_token")]
        public string UserToken;

        [JsonProperty("server_determines_connection_id")]
        public bool ServerDeterminesConnectionId;
    }
}