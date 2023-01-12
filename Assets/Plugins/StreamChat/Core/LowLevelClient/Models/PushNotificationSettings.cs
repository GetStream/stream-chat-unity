using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public class PushNotificationSettings : ModelBase, ILoadableFrom<PushNotificationSettingsInternalDTO, PushNotificationSettings>, ISavableTo<PushNotificationSettingsInternalDTO>
    {
        public bool? Disabled { get; set; }

        public System.DateTimeOffset? DisabledUntil { get; set; }

        PushNotificationSettings ILoadableFrom<PushNotificationSettingsInternalDTO, PushNotificationSettings>.LoadFromDto(PushNotificationSettingsInternalDTO dto)
        {
            Disabled = dto.Disabled;
            DisabledUntil = dto.DisabledUntil;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        PushNotificationSettingsInternalDTO ISavableTo<PushNotificationSettingsInternalDTO>.SaveToDto() =>
            new PushNotificationSettingsInternalDTO
            {
                Disabled = Disabled,
                DisabledUntil = DisabledUntil,
                AdditionalProperties = AdditionalProperties,
            };
    }
}