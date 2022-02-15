﻿using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public class OwnUser : User, ILoadableFrom<OwnUserDTO, OwnUser>
    {
        public System.Collections.Generic.ICollection<ChannelMute> ChannelMutes { get; set; }

        public System.Collections.Generic.ICollection<Device> Devices { get; set; }

        public System.Collections.Generic.ICollection<string> LatestHiddenChannels { get; set; }

        public System.Collections.Generic.ICollection<UserMute> Mutes { get; set; }

        public double? TotalUnreadCount { get; set; }

        public double? UnreadChannels { get; set; }

        public double? UnreadCount { get; set; }

        OwnUser ILoadableFrom<OwnUserDTO, OwnUser>.LoadFromDto(OwnUserDTO dto)
        {
            Banned = dto.Banned;
            ChannelMutes = ChannelMutes.TryLoadFromDtoCollection(dto.ChannelMutes);
            CreatedAt = dto.CreatedAt;
            DeactivatedAt = dto.DeactivatedAt;
            DeletedAt = dto.DeletedAt;
            Devices = Devices.TryLoadFromDtoCollection(dto.Devices);
            Id = dto.Id;
            Invisible = dto.Invisible;
            Language = dto.Language;
            LastActive = dto.LastActive;
            LatestHiddenChannels = dto.LatestHiddenChannels;
            Mutes = Mutes.TryLoadFromDtoCollection(dto.Mutes);
            Online = dto.Online;
            PushNotifications = PushNotifications.TryLoadFromDto(dto.PushNotifications);
            Role = dto.Role;
            Teams = dto.Teams;
            TotalUnreadCount = dto.TotalUnreadCount;
            UnreadChannels = dto.UnreadChannels;
            UnreadCount = dto.UnreadCount;
            UpdatedAt = dto.UpdatedAt;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}