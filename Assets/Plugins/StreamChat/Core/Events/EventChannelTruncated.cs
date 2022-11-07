﻿using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.Models;

namespace StreamChat.Core.Events
{
    public sealed class EventChannelTruncated : EventBase,
        ILoadableFrom<EventChannelTruncatedInternalDTO, EventChannelTruncated>
    {
        public Channel Channel { get; set; }

        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public string Type { get; set; }

        EventChannelTruncated ILoadableFrom<EventChannelTruncatedInternalDTO, EventChannelTruncated>.LoadFromDto(
            EventChannelTruncatedInternalDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Type = dto.Type;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}