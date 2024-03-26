using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public partial class EventTypingStop : EventBase, ILoadableFrom<TypingStopEventInternalDTO, EventTypingStop>,
        ISavableTo<TypingStopEventInternalDTO>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset CreatedAt { get; set; }

        public string ParentId { get; set; }

        public string Type { get; internal set; } = WSEventType.TypingStop;

        public User User { get; internal set; }

        EventTypingStop ILoadableFrom<TypingStopEventInternalDTO, EventTypingStop>.LoadFromDto(TypingStopEventInternalDTO dto)
        {
            AdditionalProperties = dto.AdditionalProperties;
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            ParentId = dto.ParentId;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);

            return this;
        }

        TypingStopEventInternalDTO ISavableTo<TypingStopEventInternalDTO>.SaveToDto() =>
            new TypingStopEventInternalDTO
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