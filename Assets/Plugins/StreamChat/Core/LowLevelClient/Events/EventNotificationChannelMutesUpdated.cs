using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public partial class EventNotificationChannelMutesUpdated : EventBase,
        ILoadableFrom<NotificationChannelMutesUpdatedEventInternalDTO, EventNotificationChannelMutesUpdated>
    {
        public System.DateTimeOffset? CreatedAt { get; set; }

        public OwnUser Me { get; set; }

        public string Type { get; set; }

        EventNotificationChannelMutesUpdated
            ILoadableFrom<NotificationChannelMutesUpdatedEventInternalDTO, EventNotificationChannelMutesUpdated>.
            LoadFromDto(NotificationChannelMutesUpdatedEventInternalDTO dto)
        {
            CreatedAt = dto.CreatedAt;
            Me = Me.TryLoadFromDto<OwnUserInternalDTO, OwnUser>(dto.Me);
            Type = dto.Type;

            return this;
        }
    }
}