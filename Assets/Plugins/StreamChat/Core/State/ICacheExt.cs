﻿using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State.TrackedObjects;

namespace StreamChat.Core.State
{
    internal static class ICacheExt
    {
        public static StreamChannel TryCreateOrUpdate(this ICache cache, ChannelResponseInternalDTO dto)
            => dto == null ? null : cache.Channels.CreateOrUpdate<StreamChannel, ChannelResponseInternalDTO>(dto);

        public static StreamChannelMember TryCreateOrUpdate(this ICache cache, ChannelMemberInternalDTO dto)
            => dto == null ? null : cache.ChannelMembers.CreateOrUpdate<StreamChannelMember, ChannelMemberInternalDTO>(dto);

        public static StreamUser TryCreateOrUpdate(this ICache cache, UserObjectInternalInternalDTO dto)
            => dto == null ? null : cache.Users.CreateOrUpdate<StreamUser, UserObjectInternalInternalDTO>(dto);

        public static StreamLocalUser TryCreateOrUpdate(this ICache cache, OwnUserInternalDTO dto)
            => dto == null ? null : cache.LocalUser.CreateOrUpdate<StreamLocalUser, OwnUserInternalDTO>(dto);
    }
}