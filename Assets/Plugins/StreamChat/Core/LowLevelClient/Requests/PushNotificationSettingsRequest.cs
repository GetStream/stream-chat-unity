using System;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public class PushNotificationSettingsRequest : RequestObjectBase, ISavableTo<PushNotificationSettingsRequestInternalDTO>, ISavableTo2<PushNotificationSettingsInternalDTO>
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
        
        PushNotificationSettingsInternalDTO ISavableTo2<PushNotificationSettingsInternalDTO>.SaveToDto() =>
            new PushNotificationSettingsInternalDTO
            {
                Disabled = Disabled,
                DisabledUntil = DisabledUntil,
                AdditionalProperties = AdditionalProperties
            };
    }
}