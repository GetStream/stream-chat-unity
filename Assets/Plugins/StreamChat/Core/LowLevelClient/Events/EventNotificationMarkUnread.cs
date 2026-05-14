using System;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    /// <summary>
    /// Trigger: when a channel or a thread was marked as unread by the local user from a specific message.
    /// Provides updated unread counters for the user.
    /// </summary>
    public partial class EventNotificationMarkUnread : EventBase,
        ILoadableFrom<NotificationMarkUnreadEventInternalDTO, EventNotificationMarkUnread>
    {
        public Channel Channel { get; set; }

        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        /// <summary>
        /// Id of the first unread message after the call to mark unread. Set when this is a channel mark unread (no thread).
        /// </summary>
        public string FirstUnreadMessageId { get; set; }

        public DateTimeOffset LastReadAt { get; set; }

        public string LastReadMessageId { get; set; }

        public string Team { get; set; }

        public int TotalUnreadCount { get; set; }

        public string Type { get; set; }

        public int UnreadChannels { get; set; }

        public int UnreadCount { get; set; }

        public int UnreadMessages { get; set; }

        public int UnreadThreads { get; set; }

        public User User { get; set; }

        EventNotificationMarkUnread ILoadableFrom<NotificationMarkUnreadEventInternalDTO, EventNotificationMarkUnread>.
            LoadFromDto(NotificationMarkUnreadEventInternalDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            FirstUnreadMessageId = dto.FirstUnreadMessageId;
            LastReadAt = dto.LastReadAt;
            LastReadMessageId = dto.LastReadMessageId;
            Team = dto.Team;
            TotalUnreadCount = dto.TotalUnreadCount;
            Type = dto.Type;
            UnreadChannels = dto.UnreadChannels;
            UnreadCount = dto.UnreadCount;
            UnreadMessages = dto.UnreadMessages;
            UnreadThreads = dto.UnreadThreads;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}
