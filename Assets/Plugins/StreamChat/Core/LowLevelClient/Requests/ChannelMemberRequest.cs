using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class ChannelMemberRequest : RequestObjectBase, ISavableTo<ChannelMemberRequestInternalDTO>
    {
        /// <summary>
        /// Expiration date of the ban
        /// </summary>
        public System.DateTimeOffset? BanExpires { get; set; }

        /// <summary>
        /// Whether member is banned this channel or not
        /// </summary>
        public bool? Banned { get; set; }

        /// <summary>
        /// Role of the member in the channel
        /// </summary>
        public string ChannelRole { get; set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; set; }

        public System.DateTimeOffset? DeletedAt { get; set; }

        /// <summary>
        /// Date when invite was accepted
        /// </summary>
        public System.DateTimeOffset? InviteAcceptedAt { get; set; }

        /// <summary>
        /// Date when invite was rejected
        /// </summary>
        public System.DateTimeOffset? InviteRejectedAt { get; set; }

        /// <summary>
        /// Whether member was invited or not
        /// </summary>
        public bool? Invited { get; set; }

        /// <summary>
        /// Whether member is channel moderator or not
        /// </summary>
        public bool? IsModerator { get; set; }

        /// <summary>
        /// Permission level of the member in the channel (DEPRECATED: use channel_role instead)
        /// </summary>
        public ChannelMemberRoleType? Role { get; set; }

        /// <summary>
        /// Whether member is shadow banned in this channel or not
        /// </summary>
        public bool? ShadowBanned { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; set; }

        public UserObjectRequest User { get; set; }

        public string UserId { get; set; }

        ChannelMemberRequestInternalDTO ISavableTo<ChannelMemberRequestInternalDTO>.SaveToDto() =>
            new ChannelMemberRequestInternalDTO
            {
                BanExpires = BanExpires,
                Banned = Banned,
                ChannelRole = ChannelRole,
                CreatedAt = CreatedAt,
                DeletedAt = DeletedAt,
                InviteAcceptedAt = InviteAcceptedAt,
                InviteRejectedAt = InviteRejectedAt,
                Invited = Invited,
                IsModerator = IsModerator,
                Role = Role,
                ShadowBanned = ShadowBanned,
                UpdatedAt = UpdatedAt,
                User = User.TrySaveToDto(),
                UserId = UserId,
                AdditionalProperties = AdditionalProperties,
            };
    }
}