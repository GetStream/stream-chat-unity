using StreamChat.Core.DTO.Events;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Events;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Events
{
    public partial class EventNotificationMessageNew : EventBase, ILoadableFrom<EventNotificationMessageNewDTO, EventNotificationMessageNew>
    {
        public Channel Channel { get; set; }

        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public Message Message { get; set; }

        public string Team { get; set; }

        public string Type { get; set; }

        EventNotificationMessageNew ILoadableFrom<EventNotificationMessageNewDTO, EventNotificationMessageNew>.LoadFromDto(EventNotificationMessageNewDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Message = Message.TryLoadFromDto<MessageDTO, Message>(dto.Message);
            Team = dto.Team;
            Type = dto.Type;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}