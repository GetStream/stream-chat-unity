﻿using System;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State.Caches;
using StreamChat.Core.State.TrackedObjects;

namespace StreamChat.Core.State.TrackedObjects
{
    internal sealed class StreamChannelMember : StreamTrackedObjectBase<StreamChannelMember>,
        IUpdateableFrom<ChannelMemberInternalDTO, StreamChannelMember>, IStreamChannelMember
    {
        public DateTimeOffset? BanExpires { get; private set; }

        public bool Banned { get; private set; }

        public string ChannelRole { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset? DeletedAt { get; private set; }

        public DateTimeOffset? InviteAcceptedAt { get; private set; }

        public DateTimeOffset? InviteRejectedAt { get; private set; }

        public bool Invited { get; private set; }

        public bool IsModerator { get; private set; }

        public bool ShadowBanned { get; private set; }

        public DateTimeOffset? UpdatedAt { get; private set; }

        public IStreamUser User { get; private set; }

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

        internal string UserId { get; private set; }

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