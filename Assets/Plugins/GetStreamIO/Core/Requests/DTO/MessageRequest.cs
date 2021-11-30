using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Requests.DTO
{
    /// <summary>
    /// Send message request
    /// </summary>
    public class MessageRequest
    {
        [JsonProperty("message")]
        public SendMessage Message;
    }
}