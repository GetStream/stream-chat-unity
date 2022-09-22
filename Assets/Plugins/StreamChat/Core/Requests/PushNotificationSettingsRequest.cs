using System;
using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public class PushNotificationSettingsRequest : RequestObjectBase, ISavableTo<PushNotificationSettingsRequestInternalDTO>
    {
        public bool Disabled { get; set; }

        public DateTimeOffset DisabledUntil { get; set; }

        PushNotificationSettingsRequestInternalDTO ISavableTo<PushNotificationSettingsRequestInternalDTO>.SaveToDto() =>
            new PushNotificationSettingsRequestInternalDTO
            {
                Disabled = Disabled,
                DisabledUntil = DisabledUntil,
                AdditionalProperties = AdditionalProperties
            };
    }
}