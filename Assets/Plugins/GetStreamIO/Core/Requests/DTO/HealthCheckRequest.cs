using Newtonsoft.Json;
using Plugins.GetStreamIO.Core.Events.DTO;

namespace Plugins.GetStreamIO.Core.Requests.DTO
{
    //Todo: should not inherit from BaseEvent
    public class HealthCheckRequest : BaseEvent
    {
        [JsonProperty("client_id")]
        public string ClientId;
    }
}