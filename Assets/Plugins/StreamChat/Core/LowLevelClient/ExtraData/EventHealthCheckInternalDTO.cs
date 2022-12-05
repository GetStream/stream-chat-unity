namespace StreamChat.Core.InternalDTO.Events
{
    internal partial class EventHealthCheckInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("connection_id", Required = Newtonsoft.Json.Required.Default,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ConnectionId { get; set; }
    }
}