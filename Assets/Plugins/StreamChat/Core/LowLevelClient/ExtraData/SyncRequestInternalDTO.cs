namespace StreamChat.Core.InternalDTO.Requests
{
    internal partial class SyncRequestInternalDTO
    {
        /// <summary>
        /// If set to true this will start watching requested and newly added channels that user has access to. If error occurred with this option enabled and it is not an input error - channels will still be watched.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("watch", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? Watch { get; set; }

        /// <summary>
        /// If set to true this will add 'inaccessible_cids' to response type
        /// </summary>
        [Newtonsoft.Json.JsonProperty("with_inaccessible_cids", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? WithInaccessibleCids { get; set; }
    }
}