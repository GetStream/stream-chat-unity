using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public sealed class EventUserBanned : EventBase,
        ILoadableFrom<UserBannedEventInternalDTO, EventUserBanned>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public User CreatedBy { get; set; }

        public System.DateTimeOffset? Expiration { get; set; }

        public string Reason { get; set; }

        public bool? Shadow { get; set; }

        public string Team { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventUserBanned ILoadableFrom<UserBannedEventInternalDTO, EventUserBanned>.LoadFromDto(
            UserBannedEventInternalDTO dto)
        {
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            CreatedBy = CreatedBy.TryLoadFromDto<UserObjectInternalDTO, User>(dto.CreatedBy);
            Expiration = dto.Expiration;
            Reason = dto.Reason;
            Shadow = dto.Shadow;
            Team = dto.Team;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}