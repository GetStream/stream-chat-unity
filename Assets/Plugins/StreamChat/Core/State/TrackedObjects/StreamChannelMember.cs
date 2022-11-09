using System;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.TrackedObjects
{
    public sealed class StreamChannelMember : StreamTrackedObjectBase<StreamChannelMember>,
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
        public bool Invited { get; private set; }

        /// <summary>
        /// Whether member is channel moderator or not
        /// </summary>
        public bool IsModerator { get; private set; }

        /// <summary>
        /// Whether member is shadow banned in this channel or not
        /// </summary>
        public bool ShadowBanned { get; private set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; private set; }

        public StreamUser User { get; private set; }

        public string UserId { get; private set; }

        void IUpdateableFrom<ChannelMemberInternalDTO, StreamChannelMember>.UpdateFromDto(ChannelMemberInternalDTO dto,
            ICache cache)
        {
            BanExpires = GetOrDefault(dto.BanExpires, BanExpires);
            Banned = GetOrDefault(dto.Banned, Banned);
            ChannelRole = GetOrDefault(dto.ChannelRole, ChannelRole);
            CreatedAt = GetOrDefault(dto.CreatedAt, CreatedAt);
            DeletedAt = GetOrDefault(dto.DeletedAt, DeletedAt);
            InviteAcceptedAt = GetOrDefault(dto.InviteAcceptedAt, InviteAcceptedAt);
            InviteRejectedAt = GetOrDefault(dto.InviteRejectedAt, InviteRejectedAt);
            Invited = GetOrDefault(dto.Invited, Invited);
            IsModerator = GetOrDefault(dto.IsModerator, IsModerator);
            ShadowBanned = GetOrDefault(dto.ShadowBanned, ShadowBanned);
            UpdatedAt = GetOrDefault(dto.UpdatedAt, UpdatedAt);
            User = cache.TryCreateOrUpdate(dto.User);
            UserId = GetOrDefault(dto.UserId, UserId);
        }

        internal StreamChannelMember(string uniqueId, ICacheRepository<StreamChannelMember> repository,
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