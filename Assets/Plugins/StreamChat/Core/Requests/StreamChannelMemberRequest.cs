using System;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Requests
{
    public sealed class StreamChannelMemberRequest : ISavableTo<ChannelMemberRequestInternalDTO>
    {
        /// <summary>
        /// Expiration date of the ban
        /// </summary>
        public DateTimeOffset? BanExpires { get; set; }

        /// <summary>
        /// Whether member is banned this channel or not
        /// </summary>
        public bool? Banned { get; set; }

        /// <summary>
        /// Role of the member in the channel
        /// </summary>
        public string ChannelRole { get; set; }

        /// <summary>
        /// Whether member is channel moderator or not
        /// </summary>
        public bool? IsModerator { get; set; }

        /// <summary>
        /// Whether member is shadow banned in this channel or not
        /// </summary>
        public bool? ShadowBanned { get; set; }

        ChannelMemberRequestInternalDTO ISavableTo<ChannelMemberRequestInternalDTO>.SaveToDto()
            => new ChannelMemberRequestInternalDTO
            {
                BanExpires = BanExpires,
                Banned = Banned,
                ChannelRole = ChannelRole,
                IsModerator = IsModerator,
                ShadowBanned = ShadowBanned,
            };
    }
}