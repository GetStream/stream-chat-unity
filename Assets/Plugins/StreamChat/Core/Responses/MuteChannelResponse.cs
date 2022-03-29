using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class MuteChannelResponse : ResponseObjectBase, ILoadableFrom<MuteChannelResponseDTO, MuteChannelResponse>
    {
        /// <summary>
        /// Object with channel mute (if one channel was muted)
        /// </summary>
        public ChannelMute ChannelMute { get; set; }

        /// <summary>
        /// Object with mutes (if multiple channels were muted)
        /// </summary>
        public System.Collections.Generic.ICollection<ChannelMute> ChannelMutes { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Authorized user object with fresh mutes information
        /// </summary>
        public OwnUser OwnUser { get; set; }


        MuteChannelResponse ILoadableFrom<MuteChannelResponseDTO, MuteChannelResponse>.LoadFromDto(MuteChannelResponseDTO dto)
        {
            ChannelMute = ChannelMute.TryLoadFromDto(dto.ChannelMute);
            ChannelMutes = ChannelMutes.TryLoadFromDtoCollection(dto.ChannelMutes);
            Duration = dto.Duration;
            OwnUser = OwnUser.TryLoadFromDto<OwnUserDTO, OwnUser>(dto.OwnUser);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}