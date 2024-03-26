using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public sealed class EventNotificationAddedToChannel : EventBase,
        ILoadableFrom<NotificationAddedToChannelEventInternalDTO, EventNotificationAddedToChannel>
    {
        public Channel Channel { get; set; }

        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public ChannelMember Member { get; set; }

        public string Type { get; set; }

        EventNotificationAddedToChannel
            ILoadableFrom<NotificationAddedToChannelEventInternalDTO, EventNotificationAddedToChannel>.LoadFromDto(
                NotificationAddedToChannelEventInternalDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Member = Member.TryLoadFromDto(dto.Member);
            Type = dto.Type;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}