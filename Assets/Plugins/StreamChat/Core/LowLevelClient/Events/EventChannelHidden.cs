using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public sealed class EventChannelHidden : EventBase,
        ILoadableFrom<EventChannelHiddenInternalDTO, EventChannelHidden>
    {
        public Channel Channel { get; set; }

        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public bool? ClearHistory { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventChannelHidden ILoadableFrom<EventChannelHiddenInternalDTO, EventChannelHidden>.LoadFromDto(
            EventChannelHiddenInternalDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            ClearHistory = dto.ClearHistory;
            CreatedAt = dto.CreatedAt;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}