using System;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.TrackedObjects
{
    public class StreamChannelMember : StreamTrackedObjectBase<StreamChannelMember>,
        IUpdateableFrom<ChannelMemberInternalDTO, StreamChannelMember>
    {
        /// <summary>
        /// Expiration date of the ban
        /// </summary>
        public DateTimeOffset? BanExpires { get; private set; }

        /// <summary>
        /// Whether member is banned this channel or not
        /// </summary>
        public bool? Banned { get; private set; }

        /// <summary>
        /// Role of the member in the channel
        /// </summary>
        public string ChannelRole { get; private set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public DateTimeOffset? CreatedAt { get; private set; }

        public DateTimeOffset? DeletedAt { get; private set; }

        /// <summary>
        /// Date when invite was accepted
        /// </summary>
        public DateTimeOffset? InviteAcceptedAt { get; private set; }

        /// <summary>
        /// Date when invite was rejected
        /// </summary>
        public DateTimeOffset? InviteRejectedAt { get; private set; }

        /// <summary>
        /// Whether member was invited or not
        /// </summary>
        public bool? Invited { get; private set; }

        /// <summary>
        /// Whether member is channel moderator or not
        /// </summary>
        public bool? IsModerator { get; private set; }

        /// <summary>
        /// Whether member is shadow banned in this channel or not
        /// </summary>
        public bool? ShadowBanned { get; private set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; private set; }

        public StreamUser User { get; private set; }

        public string UserId { get; private set; }

//         ChannelMember ILoadableFrom<ChannelMemberInternalDTO, ChannelMember>.LoadFromDto(ChannelMemberInternalDTO dto)
//         {
//             BanExpires = dto.BanExpires;
//             Banned = dto.Banned;
//             ChannelRole = dto.ChannelRole;
//             CreatedAt = dto.CreatedAt;
//             DeletedAt = dto.DeletedAt;
//             InviteAcceptedAt = dto.InviteAcceptedAt;
//             InviteRejectedAt = dto.InviteRejectedAt;
//             Invited = dto.Invited;
//             IsModerator = dto.IsModerator;
// #pragma warning disable 0618
//             Role = dto.Role;
// #pragma warning restore 0618
//             ShadowBanned = dto.ShadowBanned;
//             UpdatedAt = dto.UpdatedAt;
//             User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);
//             UserId = dto.UserId;
//             AdditionalProperties = dto.AdditionalProperties;
//
//             return this;
//         }
//
//         ChannelMemberInternalDTO ISavableTo<ChannelMemberInternalDTO>.SaveToDto()
//         {
//             return new ChannelMemberInternalDTO
//             {
//                 BanExpires = BanExpires,
//                 Banned = Banned,
//                 ChannelRole = ChannelRole,
//                 CreatedAt = CreatedAt,
//                 DeletedAt = DeletedAt,
//                 InviteAcceptedAt = InviteAcceptedAt,
//                 InviteRejectedAt = InviteRejectedAt,
//                 Invited = Invited,
//                 IsModerator = IsModerator,
// #pragma warning disable 0618
//                 Role = Role,
// #pragma warning restore 0618
//                 ShadowBanned = ShadowBanned,
//                 UpdatedAt = UpdatedAt,
//                 User = User.TrySaveToDto(),
//                 UserId = UserId,
//                 AdditionalProperties = AdditionalProperties,
//             };

        void IUpdateableFrom<ChannelMemberInternalDTO, StreamChannelMember>.UpdateFromDto(ChannelMemberInternalDTO dto,
            ICache cache)
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
            User = cache.TryCreateOrUpdate(dto.User);
            UserId = dto.UserId;
        }

        internal StreamChannelMember(string uniqueId, IRepository<StreamChannelMember> repository,
            ITrackedObjectContext context)
            : base(uniqueId, repository, context)
        {
        }

        protected override string InternalUniqueId
        {
            get => UserId;
            set => UserId = value;
        }

        protected override StreamChannelMember Self => this;
    }
}