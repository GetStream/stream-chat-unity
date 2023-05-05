using System;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    /// <summary>
    /// Trigger: when the total count of unread messages (across all channels the user is a member) changes
    /// Recipients: clients from the user removed that are not watching the channel
    /// </summary>
    public partial class EventNotificationMarkRead : EventBase, ILoadableFrom<NotificationMarkReadEventInternalDTO, EventNotificationMarkRead>
    {
        public Channel Channel { get; set; }

        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public string Team { get; set; }

        public int? TotalUnreadCount { get; set; }

        public string Type { get; set; }

        public int? UnreadChannels { get; set; }

        [Obsolete("Please use the TotalUnreadCount. This property will be removed in the future.")]
        public int? UnreadCount { get; set; }

        public User User { get; set; }

        EventNotificationMarkRead ILoadableFrom<NotificationMarkReadEventInternalDTO, EventNotificationMarkRead>.LoadFromDto(NotificationMarkReadEventInternalDTO dto)
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
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}