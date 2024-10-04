namespace StreamChat.Core.InternalDTO.Requests
{
    internal partial class UnmuteUserRequestInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("target_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string TargetId { get; set; }
    }
}