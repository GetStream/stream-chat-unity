namespace StreamChat.Core.InternalDTO.Requests
{
    /// <summary>
    /// Extra fields not defined in API spec
    /// </summary>
    internal partial class UserObjectRequestInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("image", Required = Newtonsoft.Json.Required.Default,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Image { get; set; }
    }
}