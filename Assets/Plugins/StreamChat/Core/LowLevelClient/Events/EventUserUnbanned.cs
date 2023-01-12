using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public sealed class EventUserUnbanned : EventBase,
        ILoadableFrom<EventUserUnbannedInternalDTO, EventUserUnbanned>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public bool? Shadow { get; set; }

        public string Team { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventUserUnbanned ILoadableFrom<EventUserUnbannedInternalDTO, EventUserUnbanned>.LoadFromDto(
            EventUserUnbannedInternalDTO dto)
        {
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Shadow = dto.Shadow;
            Team = dto.Team;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}