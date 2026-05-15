using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    /// <summary>
    /// Trigger: when a new reply is sent to a thread that the local user is a participant of
    /// </summary>
    public partial class EventNotificationThreadMessageNew : EventBase,
        ILoadableFrom<NotificationThreadMessageNewEventInternalDTO, EventNotificationThreadMessageNew>
    {
        public Channel Channel { get; set; }

        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public Message Message { get; set; }

        public string MessageId { get; set; }

        public string ParentAuthor { get; set; }

        public string Team { get; set; }

        public string ThreadId { get; set; }

        public string Type { get; set; }

        public int? UnreadThreadMessages { get; set; }

        public int? UnreadThreads { get; set; }

        public int WatcherCount { get; set; }

        EventNotificationThreadMessageNew ILoadableFrom<NotificationThreadMessageNewEventInternalDTO,
                EventNotificationThreadMessageNew>.LoadFromDto(NotificationThreadMessageNewEventInternalDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Message = Message.TryLoadFromDto<MessageResponseInternalDTO, Message>(dto.Message);
            MessageId = dto.MessageId;
            ParentAuthor = dto.ParentAuthor;
            Team = dto.Team;
            ThreadId = dto.ThreadId;
            Type = dto.Type;
            UnreadThreadMessages = dto.UnreadThreadMessages;
            UnreadThreads = dto.UnreadThreads;
            WatcherCount = dto.WatcherCount;
            AdditionalProperties = dto.AdditionalProperties;
            return this;
        }
    }
}
