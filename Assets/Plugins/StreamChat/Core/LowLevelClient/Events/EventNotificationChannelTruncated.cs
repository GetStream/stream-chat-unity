using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public sealed class EventNotificationChannelTruncated : EventBase,
        ILoadableFrom<NotificationChannelTruncatedEventInternalDTO, EventNotificationChannelTruncated>
    {
        public Channel Channel { get; set; }

        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public string Type { get; set; }

        EventNotificationChannelTruncated
            ILoadableFrom<NotificationChannelTruncatedEventInternalDTO, EventNotificationChannelTruncated>.LoadFromDto(
                NotificationChannelTruncatedEventInternalDTO dto)
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