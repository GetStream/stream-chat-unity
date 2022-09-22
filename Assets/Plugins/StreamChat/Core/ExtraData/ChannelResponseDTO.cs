namespace StreamChat.Core.InternalDTO.Responses
{
    /// <summary>
    /// Extra fields not defined in API spec
    /// </summary>
    internal partial class ChannelResponseInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
}