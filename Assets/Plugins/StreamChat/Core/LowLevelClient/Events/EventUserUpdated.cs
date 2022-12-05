using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public sealed class EventUserUpdated : EventBase,
        ILoadableFrom<EventUserUpdatedInternalDTO, EventUserUpdated>
    {
        public System.DateTimeOffset? CreatedAt { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventUserUpdated ILoadableFrom<EventUserUpdatedInternalDTO, EventUserUpdated>.LoadFromDto(
            EventUserUpdatedInternalDTO dto)
        {
            CreatedAt = dto.CreatedAt;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}