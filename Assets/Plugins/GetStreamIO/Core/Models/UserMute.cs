using GetStreamIO.Core.DTO.Models;
using Plugins.GetStreamIO.Core.Helpers;

namespace Plugins.GetStreamIO.Core.Models
{
    public partial class UserMute : ModelBase, ILoadableFrom<UserMuteDTO, UserMute>
    {
        /// <summary>
        /// Date/time of creation
        /// </summary>
        [Newtonsoft.Json.JsonProperty("created_at", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Date/time of mute expiration
        /// </summary>
        [Newtonsoft.Json.JsonProperty("expires", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? Expires { get; set; }

        /// <summary>
        /// User who's muted
        /// </summary>
        [Newtonsoft.Json.JsonProperty("target", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public User Target { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        [Newtonsoft.Json.JsonProperty("updated_at", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Owner of channel mute
        /// </summary>
        [Newtonsoft.Json.JsonProperty("user", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public User User { get; set; }

        public UserMute LoadFromDto(UserMuteDTO dto)
        {
            CreatedAt = dto.CreatedAt;
            Expires = dto.Expires;
            Target = Target.TryLoadFromDto(dto.Target);
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}