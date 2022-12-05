using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public partial class EventTypingStop : EventBase, ILoadableFrom<EventTypingStopInternalDTO, EventTypingStop>,
        ISavableTo<EventTypingStopInternalDTO>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public string ParentId { get; set; }

        public string Type { get; internal set; } = WSEventType.TypingStop;

        public User User { get; internal set; }

        EventTypingStop ILoadableFrom<EventTypingStopInternalDTO, EventTypingStop>.LoadFromDto(EventTypingStopInternalDTO dto)
        {
            AdditionalProperties = dto.AdditionalProperties;
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            ParentId = dto.ParentId;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);

            return this;
        }

        EventTypingStopInternalDTO ISavableTo<EventTypingStopInternalDTO>.SaveToDto() =>
            new EventTypingStopInternalDTO
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