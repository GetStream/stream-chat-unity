using StreamChat.Core.DTO.Models;

namespace StreamChat.Core.Models
{
    public class PushNotificationSettings : ModelBase, ILoadableFrom<PushNotificationSettingsDTO, PushNotificationSettings>, ISavableTo<PushNotificationSettingsDTO>
    {
        public bool? Disabled { get; set; }

        public System.DateTimeOffset? DisabledUntil { get; set; }

        PushNotificationSettings ILoadableFrom<PushNotificationSettingsDTO, PushNotificationSettings>.LoadFromDto(PushNotificationSettingsDTO dto)
        {
            Disabled = dto.Disabled;
            DisabledUntil = dto.DisabledUntil;

            return this;
        }

        PushNotificationSettingsDTO ISavableTo<PushNotificationSettingsDTO>.SaveToDto() =>
            new PushNotificationSettingsDTO
            {
                Disabled = Disabled,
                DisabledUntil = DisabledUntil,
                AdditionalProperties = AdditionalProperties,
            };
    }
}