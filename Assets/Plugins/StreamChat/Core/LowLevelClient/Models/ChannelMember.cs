using System;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public class ChannelMember : ModelBase, ILoadableFrom<ChannelMemberInternalDTO, ChannelMember>, ISavableTo<ChannelMemberInternalDTO>
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
        /// Date/time of creation
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }

        /// <summary>
        /// Date when invite was accepted
        /// </summary>
        public DateTimeOffset? InviteAcceptedAt { get; set; }

        /// <summary>
        /// Date when invite was rejected
        /// </summary>
        public DateTimeOffset? InviteRejectedAt { get; set; }

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
        [Obsolete("Use ChannelRole instead")]
        public ChannelMemberRoleType? Role { get; set; }

        /// <summary>
        /// Whether member is shadow banned in this channel or not
        /// </summary>
        public bool? ShadowBanned { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }

        ChannelMember ILoadableFrom<ChannelMemberInternalDTO, ChannelMember>.LoadFromDto(ChannelMemberInternalDTO dto)
        {
            BanExpires = dto.BanExpires;
            Banned = dto.Banned;
            ChannelRole = dto.ChannelRole;
            CreatedAt = dto.CreatedAt;
            DeletedAt = dto.DeletedAt;
            InviteAcceptedAt = dto.InviteAcceptedAt;
            InviteRejectedAt = dto.InviteRejectedAt;
            Invited = dto.Invited;
            IsModerator = dto.IsModerator;
#pragma warning disable 0618
            Role = dto.Role;
#pragma warning restore 0618
            ShadowBanned = dto.ShadowBanned;
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            UserId = dto.UserId;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        ChannelMemberInternalDTO ISavableTo<ChannelMemberInternalDTO>.SaveToDto()
        {
            return new ChannelMemberInternalDTO
            {
                BanExpires = BanExpires,
                Banned = Banned.GetValueOrDefault(),
                ChannelRole = ChannelRole,
                CreatedAt = CreatedAt.GetValueOrDefault(),
                DeletedAt = DeletedAt,
                InviteAcceptedAt = InviteAcceptedAt,
                InviteRejectedAt = InviteRejectedAt,
                Invited = Invited,
                IsModerator = IsModerator,
#pragma warning disable 0618
                Role = Role,
#pragma warning restore 0618
                ShadowBanned = ShadowBanned.GetValueOrDefault(),
                UpdatedAt = UpdatedAt.GetValueOrDefault(),
                User = User.TrySaveToDto(),
                UserId = UserId,
                AdditionalProperties = AdditionalProperties,
            };
        }
    }
}