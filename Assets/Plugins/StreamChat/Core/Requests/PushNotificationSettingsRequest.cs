using System;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public class PushNotificationSettingsRequest : RequestObjectBase, ISavableTo<PushNotificationSettingsRequestDTO>,
        ISavableTo<PushNotificationSettingsDTO>
    {
        public bool Disabled { get; set; }

        public DateTimeOffset DisabledUntil { get; set; }

        PushNotificationSettingsRequestDTO ISavableTo<PushNotificationSettingsRequestDTO>.SaveToDto()
            => new PushNotificationSettingsRequestDTO
            {
                Disabled = Disabled,
                DisabledUntil = DisabledUntil,
                AdditionalProperties = AdditionalProperties
            };

        PushNotificationSettingsDTO ISavableTo<PushNotificationSettingsDTO>.SaveToDto()
            => new PushNotificationSettingsDTO
            {
                Disabled = Disabled,
                DisabledUntil = DisabledUntil,
                AdditionalProperties = AdditionalProperties
            };
    }
}