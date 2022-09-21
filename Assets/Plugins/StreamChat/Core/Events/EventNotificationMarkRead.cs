using System;
using StreamChat.Core.DTO.Events;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Events
{
    /// <summary>
    /// Trigger: when the total count of unread messages (across all channels the user is a member) changes
    /// Recipients: clients from the user removed that are not watching the channel
    /// </summary>
    public partial class EventNotificationMarkRead : EventBase, ILoadableFrom<EventNotificationMarkReadDTO, EventNotificationMarkRead>
    {
        public Channel Channel { get; set; }

        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public string Team { get; set; }

        public int? TotalUnreadCount { get; set; }

        public string Type { get; set; }

        public int? UnreadChannels { get; set; }

        [Obsolete("Please use the TotalUnreadCount. This property will be removed in the future.")]
        public int? UnreadCount { get; set; }

        public User User { get; set; }

        EventNotificationMarkRead ILoadableFrom<EventNotificationMarkReadDTO, EventNotificationMarkRead>.LoadFromDto(EventNotificationMarkReadDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Team = dto.Team;
            TotalUnreadCount = dto.TotalUnreadCount;
            Type = dto.Type;
            UnreadChannels = dto.UnreadChannels;
#pragma warning disable 0618
            UnreadCount = dto.TotalUnreadCount;
#pragma warning restore 0618
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}