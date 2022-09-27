﻿using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Events
{
    public partial class EventTypingStart : EventBase, ILoadableFrom<EventTypingStartInternalDTO, EventTypingStart>,
        ISavableTo<EventTypingStartInternalDTO>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public string ParentId { get; set; }

        public string Type { get; internal set; } = EventType.TypingStart;

        public User User { get; internal set; }

        EventTypingStart ILoadableFrom<EventTypingStartInternalDTO, EventTypingStart>.LoadFromDto(EventTypingStartInternalDTO dto)
        {
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            ParentId = dto.ParentId;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        EventTypingStartInternalDTO ISavableTo<EventTypingStartInternalDTO>.SaveToDto() =>
            new EventTypingStartInternalDTO
            {
                ChannelId = ChannelId,
                ChannelType = ChannelType,
                Cid = Cid,
                CreatedAt = CreatedAt,
                ParentId = ParentId,
                Type = Type,
                User = User.TrySaveToDto(),
                AdditionalProperties = AdditionalProperties,
            };
    }
}