using System;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public class ChannelMember : ModelBase, ILoadableFrom<ChannelMemberDTO, ChannelMember>, ISavableTo<ChannelMemberDTO>
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
        /// Whether member is shadow banned in this channel or not
        /// </summary>
        public bool? ShadowBanned { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }

        ChannelMember ILoadableFrom<ChannelMemberDTO, ChannelMember>.LoadFromDto(ChannelMemberDTO dto)
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
            ShadowBanned = dto.ShadowBanned;
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            UserId = dto.UserId;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        ChannelMemberDTO ISavableTo<ChannelMemberDTO>.SaveToDto()
        {
            return new ChannelMemberDTO
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
                ShadowBanned = ShadowBanned,
                UpdatedAt = UpdatedAt,
                User = User.TrySaveToDto(),
                UserId = UserId,
                AdditionalProperties = AdditionalProperties,
            };
        }
    }
}