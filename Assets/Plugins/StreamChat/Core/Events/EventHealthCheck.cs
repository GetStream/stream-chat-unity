using StreamChat.Core.DTO.Events;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Events
{
    public partial class EventHealthCheck : EventBase, ILoadableFrom<EventHealthCheckDTO, EventHealthCheck>
    {
        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public OwnUser Me { get; set; }

        public string Type { get; set; }

        //Not in OpenAPI syntax but mentioned in docs
        public string ConnectionId { get; set; }

        EventHealthCheck ILoadableFrom<EventHealthCheckDTO, EventHealthCheck>.LoadFromDto(EventHealthCheckDTO dto)
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

namespace StreamChat.Core.DTO.Events
{
    internal partial class EventHealthCheckDTO
    {
        [Newtonsoft.Json.JsonProperty("connection_id", Required = Newtonsoft.Json.Required.Default,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ConnectionId { get; set; }
    }
}