namespace StreamChat.Core.InternalDTO.Responses
{
    internal partial class FullUserResponseInternalDTO
    {
        /// <summary>
        /// Expiration date of the ban
        /// </summary>
        [Newtonsoft.Json.JsonProperty("ban_expires", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? BanExpires { get; set; }
    }
}