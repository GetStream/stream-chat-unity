using GetStreamIO.Core.DTO.Events;
using GetStreamIO.Core.DTO.Models;
using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core.Events.V2
{
    public partial class EventHealthCheck : EventBase, ILoadableFrom<EventHealthCheckDTO, EventHealthCheck>
    {
        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public OwnUser Me { get; set; }

        public string Type { get; set; }

        //Not in OpenAPI syntax but mentioned in docs
        public string ConnectionId { get; set; }

        public EventHealthCheck LoadFromDto(EventHealthCheckDTO dto)
        {
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Me = Me.TryLoadFromDto<OwnUserDTO, OwnUser>(dto.Me);
            Type = dto.Type;
            AdditionalProperties = dto.AdditionalProperties;

            ConnectionId = dto.ConnectionId;

            return this;
        }
    }
}

namespace GetStreamIO.Core.DTO.Events
{
    public partial class EventHealthCheckDTO
    {
        [Newtonsoft.Json.JsonProperty("connection_id", Required = Newtonsoft.Json.Required.Default,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ConnectionId { get; set; }
    }
}