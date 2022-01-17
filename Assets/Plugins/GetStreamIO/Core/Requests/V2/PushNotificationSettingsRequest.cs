using System;
using GetStreamIO.Core.DTO.Requests;

namespace Plugins.GetStreamIO.Core.Requests.V2
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