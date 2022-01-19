using GetStreamIO.Core.DTO.Models;
using GetStreamIO.Core.DTO.Responses;
using Plugins.GetStreamIO.Core.Helpers;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Utils;

namespace Plugins.GetStreamIO.Core.Responses
{
    public partial class MuteUserResponse : ResponseObjectBase, ILoadableFrom<MuteUserResponseDTO, MuteUserResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Object with user mute (if one user was muted)
        /// </summary>
        public UserMute Mute { get; set; }

        /// <summary>
        /// Object with mutes (if multiple users were muted)
        /// </summary>
        public System.Collections.Generic.ICollection<UserMute> Mutes { get; set; }

        /// <summary>
        /// Authorized user object with fresh mutes information
        /// </summary>
        public OwnUser OwnUser { get; set; }

        public MuteUserResponse LoadFromDto(MuteUserResponseDTO dto)
        {
            Duration = dto.Duration;
            Mute = Mute.TryLoadFromDto(dto.Mute);
            Mutes = Mutes.TryLoadFromDtoCollection(dto.Mutes);
            OwnUser = OwnUser.TryLoadFromDto<OwnUserDTO, OwnUser>(dto.OwnUser);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}