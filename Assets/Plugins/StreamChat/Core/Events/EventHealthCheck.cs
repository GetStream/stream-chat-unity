﻿using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Events
{
    public partial class EventHealthCheck : EventBase, ILoadableFrom<EventHealthCheckInternalDTO, EventHealthCheck>
    {
        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public OwnUser Me { get; set; }

        public string Type { get; set; }

        //Not in OpenAPI syntax but mentioned in docs
        public string ConnectionId { get; set; }

        EventHealthCheck ILoadableFrom<EventHealthCheckInternalDTO, EventHealthCheck>.LoadFromDto(EventHealthCheckInternalDTO dto)
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

namespace StreamChat.Core.InternalDTO.Events
{
    internal partial class EventHealthCheckInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("connection_id", Required = Newtonsoft.Json.Required.Default,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ConnectionId { get; set; }
    }
}