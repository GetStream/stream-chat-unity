namespace StreamChat.Core.InternalDTO.Requests
{
    internal partial class UpdateUserPartialRequestEntryInternalDTO
    {
        /// <summary>
        /// User ID to update
        /// </summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// Sets new field values
        /// </summary>
        [Newtonsoft.Json.JsonProperty("set", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.Dictionary<string, object> Set { get; set; }

        /// <summary>
        /// Array of field names to unset
        /// </summary>
        [Newtonsoft.Json.JsonProperty("unset", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<string> Unset { get; set; }

        private System.Collections.Generic.Dictionary<string, object> _additionalProperties;

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.Dictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }

    }
}