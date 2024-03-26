using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public sealed class EventUserPresenceChanged : EventBase,
        ILoadableFrom<UserPresenceChangedEventInternalDTO, EventUserPresenceChanged>
    {
        public System.DateTimeOffset? CreatedAt { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventUserPresenceChanged ILoadableFrom<UserPresenceChangedEventInternalDTO, EventUserPresenceChanged>.
            LoadFromDto(UserPresenceChangedEventInternalDTO dto)
        {
            CreatedAt = dto.CreatedAt;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}