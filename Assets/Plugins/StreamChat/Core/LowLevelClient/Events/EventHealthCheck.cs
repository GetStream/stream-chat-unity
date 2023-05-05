using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public partial class EventHealthCheck : EventBase, ILoadableFrom<HealthCheckEventInternalDTO, EventHealthCheck>
    {
        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public OwnUser Me { get; set; }

        public string Type { get; set; }

        //Not in OpenAPI syntax but mentioned in docs
        public string ConnectionId { get; set; }

        EventHealthCheck ILoadableFrom<HealthCheckEventInternalDTO, EventHealthCheck>.LoadFromDto(HealthCheckEventInternalDTO dto)
        {
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Me = Me.TryLoadFromDto<OwnUserInternalDTO, OwnUser>(dto.Me);
            Type = dto.Type;
            AdditionalProperties = dto.AdditionalProperties;

            ConnectionId = dto.ConnectionId;

            return this;
        }
    }
}