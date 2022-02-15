using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
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

        MuteUserResponse ILoadableFrom<MuteUserResponseDTO, MuteUserResponse>.LoadFromDto(MuteUserResponseDTO dto)
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