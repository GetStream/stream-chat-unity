using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Models
{

    public class PushNotificationSettings : ModelBase,
        ILoadableFrom<PushNotificationSettingsInternalDTO, PushNotificationSettings>,
        ILoadableFrom<PushNotificationSettingsRequestInternalDTO, PushNotificationSettings>,
        ISavableTo<PushNotificationSettingsInternalDTO>, ISavableTo<PushNotificationSettingsRequestInternalDTO>
    {
        public bool? Disabled { get; set; }

        public System.DateTimeOffset? DisabledUntil { get; set; }

        PushNotificationSettings ILoadableFrom<PushNotificationSettingsInternalDTO, PushNotificationSettings>.
            LoadFromDto(PushNotificationSettingsInternalDTO dto)
        {
            Disabled = dto.Disabled;
            DisabledUntil = dto.DisabledUntil;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
        
        PushNotificationSettings ILoadableFrom<PushNotificationSettingsRequestInternalDTO, PushNotificationSettings>.
            LoadFromDto(PushNotificationSettingsRequestInternalDTO dto)
        {
            Disabled = dto.Disabled;
            DisabledUntil = dto.DisabledUntil;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        PushNotificationSettingsInternalDTO ISavableTo<PushNotificationSettingsInternalDTO>.SaveToDto()
            => new PushNotificationSettingsInternalDTO
            {
                Disabled = Disabled,
                DisabledUntil = DisabledUntil,
                AdditionalProperties = AdditionalProperties,
            };
        
        PushNotificationSettingsRequestInternalDTO ISavableTo<PushNotificationSettingsRequestInternalDTO>.SaveToDto()
            => new PushNotificationSettingsRequestInternalDTO
            {
                Disabled = Disabled,
                DisabledUntil = DisabledUntil,
                AdditionalProperties = AdditionalProperties,
            };
    }
}