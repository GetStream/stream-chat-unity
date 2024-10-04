namespace StreamChat.Core.InternalDTO.Requests
{
    internal partial class UpdateUserPartialRequestInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("users", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<UpdateUserPartialRequestEntryInternalDTO> Users { get; set; }

    }
}