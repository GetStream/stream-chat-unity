using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Models;

namespace StreamChat.Core.Events
{
    public sealed class EventNotificationMutesUpdated : EventBase,
        ILoadableFrom<EventNotificationMutesUpdatedInternalDTO, EventNotificationMutesUpdated>
    {
        public System.DateTimeOffset? CreatedAt { get; set; }

        public OwnUser Me { get; set; }

        public string Type { get; set; }

        EventNotificationMutesUpdated
            ILoadableFrom<EventNotificationMutesUpdatedInternalDTO, EventNotificationMutesUpdated>.LoadFromDto(
                EventNotificationMutesUpdatedInternalDTO dto)
        {
            CreatedAt = dto.CreatedAt;
            Me = Me.TryLoadFromDto<OwnUserInternalDTO, OwnUser>(dto.Me);
            Type = dto.Type;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}