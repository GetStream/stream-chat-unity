using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class MuteChannelResponse : ResponseObjectBase, ILoadableFrom<MuteChannelResponseInternalDTO, MuteChannelResponse>
    {
        /// <summary>
        /// Object with channel mute (if one channel was muted)
        /// </summary>
        public ChannelMute ChannelMute { get; set; }

        /// <summary>
        /// Object with mutes (if multiple channels were muted)
        /// </summary>
        public System.Collections.Generic.List<ChannelMute> ChannelMutes { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Authorized user object with fresh mutes information
        /// </summary>
        public OwnUser OwnUser { get; set; }


        MuteChannelResponse ILoadableFrom<MuteChannelResponseInternalDTO, MuteChannelResponse>.LoadFromDto(MuteChannelResponseInternalDTO dto)
        {
            ChannelMute = ChannelMute.TryLoadFromDto(dto.ChannelMute);
            ChannelMutes = ChannelMutes.TryLoadFromDtoCollection(dto.ChannelMutes);
            Duration = dto.Duration;
            OwnUser = OwnUser.TryLoadFromDto<OwnUserInternalDTO, OwnUser>(dto.OwnUser);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}