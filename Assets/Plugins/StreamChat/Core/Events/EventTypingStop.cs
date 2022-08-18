using StreamChat.Core.DTO.Events;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Events
{
    public partial class EventTypingStop : EventBase, ILoadableFrom<EventTypingStopDTO, EventTypingStop>,
        ISavableTo<EventTypingStopDTO>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public string ParentId { get; set; }

        public string Type { get; internal set; } = EventType.TypingStop;

        public User User { get; internal set; }

        EventTypingStop ILoadableFrom<EventTypingStopDTO, EventTypingStop>.LoadFromDto(EventTypingStopDTO dto)
        {
            AdditionalProperties = dto.AdditionalProperties;
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            ParentId = dto.ParentId;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);

            return this;
        }

        EventTypingStopDTO ISavableTo<EventTypingStopDTO>.SaveToDto() =>
            new EventTypingStopDTO
            {
                ChannelId = ChannelId,
                ChannelType = ChannelType,
                Cid = Cid,
                CreatedAt = CreatedAt,
                ParentId = ParentId,
                Type = Type,
                User = User.TrySaveToDto(),
                AdditionalProperties = AdditionalProperties,
            };
    }
}