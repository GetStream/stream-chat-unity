using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Models
{
    public class StreamPushNotificationSettings : IStateLoadableFrom<PushNotificationSettingsInternalDTO,
        StreamPushNotificationSettings>
    {
        public bool Disabled { get; private set; }

        public System.DateTimeOffset DisabledUntil { get; private set; }

        StreamPushNotificationSettings
            IStateLoadableFrom<PushNotificationSettingsInternalDTO, StreamPushNotificationSettings>.LoadFromDto(
                PushNotificationSettingsInternalDTO dto, ICache cache)
        {
            Disabled = dto.Disabled.GetValueOrDefault();
            DisabledUntil = dto.DisabledUntil.GetValueOrDefault();

            return this;
        }
    }
}