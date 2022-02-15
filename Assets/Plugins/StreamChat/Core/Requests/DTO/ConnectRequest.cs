using Newtonsoft.Json;

namespace StreamChat.Core.Requests.DTO
{
    internal class ConnectRequest
    {
        [JsonProperty("Json")]
        public string Json { get; set; }

        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [JsonProperty("authorization")]
        public string UserToken { get; set; }

        [JsonProperty("stream-auth-type")]
        public string StreamAuthType { get; set; }
    }
}