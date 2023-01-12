﻿using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public sealed class EventUserWatchingStart : EventBase,
        ILoadableFrom<EventUserWatchingStartInternalDTO, EventUserWatchingStart>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public string Team { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        public int? WatcherCount { get; set; }

        EventUserWatchingStart ILoadableFrom<EventUserWatchingStartInternalDTO, EventUserWatchingStart>.LoadFromDto(
            EventUserWatchingStartInternalDTO dto)
        {
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Team = dto.Team;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);
            WatcherCount = dto.WatcherCount;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}