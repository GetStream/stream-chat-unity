using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State.TrackedObjects;

namespace StreamChat.Core.State
{
    internal static class ICacheExt
    {
        public static StreamMessage TryCreateOrUpdate(this ICache cache, MessageInternalDTO dto)
            => dto == null ? null : cache.Messages.CreateOrUpdate<StreamMessage, MessageInternalDTO>(dto, out _);

        public static StreamMessage TryCreateOrUpdate(this ICache cache, MessageInternalDTO dto, out bool wasCreated)
        {
            wasCreated = false;
            return dto == null ? null : cache.Messages.CreateOrUpdate<StreamMessage, MessageInternalDTO>(dto, out wasCreated);
        }

        public static StreamChannel TryCreateOrUpdate(this ICache cache, ChannelResponseInternalDTO dto)
            => dto == null ? null : cache.Channels.CreateOrUpdate<StreamChannel, ChannelResponseInternalDTO>(dto, out _);

        public static StreamChannel TryCreateOrUpdate(this ICache cache, ChannelStateResponseFieldsInternalDTO dto)
            => dto == null ? null : cache.Channels.CreateOrUpdate<StreamChannel, ChannelStateResponseFieldsInternalDTO>(dto, out _);

        public static StreamChannelMember TryCreateOrUpdate(this ICache cache, ChannelMemberInternalDTO dto)
            => dto == null ? null : cache.ChannelMembers.CreateOrUpdate<StreamChannelMember, ChannelMemberInternalDTO>(dto, out _);

        public static StreamUser TryCreateOrUpdate(this ICache cache, UserResponseInternalDTO dto)
            => dto == null ? null : cache.Users.CreateOrUpdate<StreamUser, UserResponseInternalDTO>(dto, out _);

        public static StreamUser TryCreateOrUpdate(this ICache cache, UserObjectInternalInternalDTO dto)
            => dto == null ? null : cache.Users.CreateOrUpdate<StreamUser, UserObjectInternalInternalDTO>(dto, out _);

        public static StreamLocalUser TryCreateOrUpdate(this ICache cache, OwnUserInternalDTO dto)
            => dto == null ? null : cache.LocalUser.CreateOrUpdate<StreamLocalUser, OwnUserInternalDTO>(dto, out _);
    }
}