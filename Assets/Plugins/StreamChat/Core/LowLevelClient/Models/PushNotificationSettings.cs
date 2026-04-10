using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Models
{
    
    public class PushNotificationSettings : ModelBase,
        ILoadableFrom<PushNotificationSettingsInternalDTO, PushNotificationSettings>,
        ILoadableFrom2<PushNotificationSettingsRequestInternalDTO, PushNotificationSettings>,
        ILoadableFrom3<PushNotificationSettingsResponseInternalDTO, PushNotificationSettings>,
        ISavableTo<PushNotificationSettingsInternalDTO>, ISavableTo2<PushNotificationSettingsRequestInternalDTO>
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
        
        PushNotificationSettings ILoadableFrom2<PushNotificationSettingsRequestInternalDTO, PushNotificationSettings>.
            LoadFromDto(PushNotificationSettingsRequestInternalDTO dto)
        {
            Disabled = dto.Disabled;
            DisabledUntil = dto.DisabledUntil;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
        
        PushNotificationSettings ILoadableFrom3<PushNotificationSettingsResponseInternalDTO, PushNotificationSettings>.
            LoadFromDto(PushNotificationSettingsResponseInternalDTO dto)
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
        
        PushNotificationSettingsRequestInternalDTO ISavableTo2<PushNotificationSettingsRequestInternalDTO>.SaveToDto()
            => new PushNotificationSettingsRequestInternalDTO
            {
                Disabled = Disabled,
                DisabledUntil = DisabledUntil,
                AdditionalProperties = AdditionalProperties,
            };
    }
}