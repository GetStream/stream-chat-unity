using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class MuteUserResponse : ResponseObjectBase, ILoadableFrom<MuteUserResponseInternalDTO, MuteUserResponse>
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
        public System.Collections.Generic.List<UserMute> Mutes { get; set; }

        /// <summary>
        /// Authorized user object with fresh mutes information
        /// </summary>
        public OwnUser OwnUser { get; set; }

        MuteUserResponse ILoadableFrom<MuteUserResponseInternalDTO, MuteUserResponse>.LoadFromDto(MuteUserResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            Mute = Mute.TryLoadFromDto(dto.Mute);
            Mutes = Mutes.TryLoadFromDtoCollection(dto.Mutes);
            OwnUser = OwnUser.TryLoadFromDto<OwnUserInternalDTO, OwnUser>(dto.OwnUser);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}