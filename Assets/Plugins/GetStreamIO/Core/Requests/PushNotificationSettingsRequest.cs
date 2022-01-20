using System;
using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public class PushNotificationSettingsRequest : RequestObjectBase, ISavableTo<PushNotificationSettingsRequestDTO>
    {
        public bool Disabled { get; set; }

        public DateTimeOffset DisabledUntil { get; set; }

        public PushNotificationSettingsRequestDTO SaveToDto() =>
            new PushNotificationSettingsRequestDTO
            {
                Disabled = Disabled,
                DisabledUntil = DisabledUntil,
                AdditionalProperties = AdditionalProperties
            };
    }
}