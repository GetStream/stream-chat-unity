using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Models
{
    public class StreamPushNotificationSettings :
        IStateLoadableFrom<PushNotificationSettingsInternalDTO, StreamPushNotificationSettings>,
        IStateLoadableFrom<PushNotificationSettingsResponseInternalDTO, StreamPushNotificationSettings>
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

        StreamPushNotificationSettings
            IStateLoadableFrom<PushNotificationSettingsResponseInternalDTO, StreamPushNotificationSettings>.LoadFromDto(
                PushNotificationSettingsResponseInternalDTO dto, ICache cache)
        {
            Disabled = dto.Disabled.GetValueOrDefault();
            DisabledUntil = dto.DisabledUntil.GetValueOrDefault();

            return this;
        }
    }
}