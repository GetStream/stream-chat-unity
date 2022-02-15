using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public class UserMute : ModelBase, ILoadableFrom<UserMuteDTO, UserMute>
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

        UserMute ILoadableFrom<UserMuteDTO, UserMute>.LoadFromDto(UserMuteDTO dto)
        {
            CreatedAt = dto.CreatedAt;
            Expires = dto.Expires;
            Target = Target.TryLoadFromDto<UserObjectDTO, User>(dto.Target);
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}