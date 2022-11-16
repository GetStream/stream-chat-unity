using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Models
{
    public class StreamChannelMute : IStateLoadableFrom<ChannelMuteInternalDTO, StreamChannelMute>
    {
        public IStreamChannel Channel { get; private set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; private set; }

        /// <summary>
        /// Date/time of mute expiration
        /// </summary>
        public System.DateTimeOffset? Expires { get; private set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; private set; }

        /// <summary>
        /// Owner of channel mute
        /// </summary>
        public IStreamUser User { get; private set; }

        StreamChannelMute IStateLoadableFrom<ChannelMuteInternalDTO, StreamChannelMute>.LoadFromDto(ChannelMuteInternalDTO dto, ICache cache)
        {
            Channel = cache.TryCreateOrUpdate(dto.Channel);
            CreatedAt = dto.CreatedAt;
            Expires = dto.Expires;
            UpdatedAt = dto.UpdatedAt;
            User = cache.TryCreateOrUpdate(dto.User);

            return this;
        }
    }
}