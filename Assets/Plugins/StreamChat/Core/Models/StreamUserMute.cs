using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Models
{
    public class StreamUserMute : IStateLoadableFrom<UserMuteInternalDTO, StreamUserMute>
    {
        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; private set; }

        /// <summary>
        /// Date/time of mute expiration
        /// </summary>
        public System.DateTimeOffset? Expires { get; private set; }

        /// <summary>
        /// User who's muted
        /// </summary>
        public User Target { get; private set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; private set; }

        /// <summary>
        /// Owner of channel mute
        /// </summary>
        public User User { get; private set; }

        StreamUserMute IStateLoadableFrom<UserMuteInternalDTO, StreamUserMute>.LoadFromDto(UserMuteInternalDTO dto, ICache cache)
        {
            CreatedAt = dto.CreatedAt;
            Expires = dto.Expires;
            Target = Target.TryLoadFromDto<UserObjectInternalDTO, User>(dto.Target);
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);

            return this;
        }
    }
}