using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Models;

namespace StreamChat.Core.Events
{
    public sealed class EventUserPresenceChanged : EventBase,
        ILoadableFrom<EventUserPresenceChangedInternalDTO, EventUserPresenceChanged>
    {
        public System.DateTimeOffset? CreatedAt { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventUserPresenceChanged ILoadableFrom<EventUserPresenceChangedInternalDTO, EventUserPresenceChanged>.
            LoadFromDto(EventUserPresenceChangedInternalDTO dto)
        {
            CreatedAt = dto.CreatedAt;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}