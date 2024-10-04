namespace StreamChat.Core.InternalDTO.Responses
{
    internal partial class MessageResponseInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("before_message_send_failed", Required = Newtonsoft.Json.Required.Default,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? BeforeMessageSendFailed { get; set; }
    }
}