﻿using StreamChat.Core.DTO.Events;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Events
{
    public partial class EventUserFlagged : EventBase, ILoadableFrom<EventUserFlaggedInternalDTO, EventUserFlagged>
    {
        public System.DateTimeOffset? CreatedAt { get; set; }

        public string TargetUser { get; set; }

        public System.Collections.Generic.List<string> TargetUsers { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventUserFlagged ILoadableFrom<EventUserFlaggedInternalDTO, EventUserFlagged>.LoadFromDto(EventUserFlaggedInternalDTO dto)
        {
            CreatedAt = dto.CreatedAt;
            TargetUser = dto.TargetUser;
            TargetUsers = dto.TargetUsers;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}