using GetStreamIO.Core.DTO.Models;

namespace Plugins.GetStreamIO.Core.Models
{
    public partial class ChannelMute : ModelBase, ILoadableFrom<ChannelMuteDTO, ChannelMute>
    {
        public Channel Channel { get; set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Date/time of mute expiration
        /// </summary>
        public System.DateTimeOffset? Expires { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Owner of channel mute
        /// </summary>
        public User User { get; set; }

        public ChannelMute LoadFromDto(ChannelMuteDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            CreatedAt = dto.CreatedAt;
            Expires = dto.Expires;
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}