﻿using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Models;
using StreamChat.Core.State;

namespace StreamChat.Core.State.Models
{
    public class StreamUserMute : IStateLoadableFrom<UserMuteInternalDTO, StreamUserMute>
    {
        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Date/time of mute expiration
        /// </summary>
        public System.DateTimeOffset? Expires { get; set; }

        /// <summary>
        /// User who's muted
        /// </summary>
        public User Target { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Owner of channel mute
        /// </summary>
        public User User { get; set; }

        StreamUserMute IStateLoadableFrom<UserMuteInternalDTO, StreamUserMute>.LoadFromDto(UserMuteInternalDTO dto, ICache cache)
        {
            CreatedAt = dto.CreatedAt;
            Expires = dto.Expires;
            Target = Target.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.Target);
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);

            return this;
        }
    }
}