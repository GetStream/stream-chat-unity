namespace StreamChat.Core.InternalDTO.Models
{
    internal partial class UserEventPayloadInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("ban_expires", Required = Newtonsoft.Json.Required.Default,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? BanExpires { get; set; }

        [Newtonsoft.Json.JsonProperty("push_notifications", Required = Newtonsoft.Json.Required.Default,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public PushNotificationSettingsInternalDTO PushNotifications { get; set; }
        
        [Newtonsoft.Json.JsonProperty("shadow_banned", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool ShadowBanned { get; set; }
    }
}