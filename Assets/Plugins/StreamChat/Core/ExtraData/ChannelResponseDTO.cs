namespace StreamChat.Core.DTO.Responses
{
    /// <summary>
    /// Extra fields not defined in API spec
    /// </summary>
    internal partial class ChannelResponseDTO
    {
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
}