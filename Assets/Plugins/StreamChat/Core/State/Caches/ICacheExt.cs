using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.State.Caches
{
    internal static class ICacheExt
    {
        public static StreamMessage TryCreateOrUpdate(this ICache cache, MessageInternalDTO dto)
            => dto == null ? null : cache.Messages.CreateOrUpdate<StreamMessage, MessageInternalDTO>(dto, out _);
        
        public static StreamMessage TryCreateOrUpdate(this ICache cache, MessageResponseInternalDTO dto)
            => dto == null ? null : cache.Messages.CreateOrUpdate2<StreamMessage, MessageResponseInternalDTO>(dto, out _);

        public static StreamMessage TryCreateOrUpdate(this ICache cache, MessageInternalDTO dto, out bool wasCreated)
        {
            wasCreated = false;
            return dto == null
                ? null
                : cache.Messages.CreateOrUpdate<StreamMessage, MessageInternalDTO>(dto, out wasCreated);
        }

        public static StreamChannel TryCreateOrUpdate(this ICache cache, ChannelResponseInternalDTO dto)
            => dto == null
                ? null
                : cache.Channels.CreateOrUpdate2<StreamChannel, ChannelResponseInternalDTO>(dto, out _);
        
        public static StreamChannel TryCreateOrUpdate(this ICache cache, ChannelResponseInternalDTO dto, out bool wasCreated)
        {
            wasCreated = false;
            return dto == null
                ? null
                : cache.Channels.CreateOrUpdate2<StreamChannel, ChannelResponseInternalDTO>(dto, out wasCreated);
        }

        public static StreamChannel TryCreateOrUpdate(this ICache cache, ChannelStateResponseFieldsInternalDTO dto)
            => dto == null
                ? null
                : cache.Channels.CreateOrUpdate3<StreamChannel, ChannelStateResponseFieldsInternalDTO>(dto, out _);

        public static StreamChannel TryCreateOrUpdate(this ICache cache, ChannelStateResponseInternalDTO dto)
            => dto == null
                ? null
                : cache.Channels.CreateOrUpdate<StreamChannel, ChannelStateResponseInternalDTO>(dto, out _);

        public static StreamChannel TryCreateOrUpdate(this ICache cache, UpdateChannelResponseInternalDTO dto)
            => dto == null
                ? null
                : cache.Channels.CreateOrUpdate4<StreamChannel, UpdateChannelResponseInternalDTO>(dto, out _);

        public static StreamChannelMember TryCreateOrUpdate(this ICache cache, ChannelMemberInternalDTO dto)
            => dto == null
                ? null
                : cache.ChannelMembers.CreateOrUpdate<StreamChannelMember, ChannelMemberInternalDTO>(dto, out _);

        public static StreamUser TryCreateOrUpdate(this ICache cache, UserResponseInternalDTO dto)
            => dto == null ? null : cache.Users.CreateOrUpdate2<StreamUser, UserResponseInternalDTO>(dto, out _);
        
        public static StreamUser TryCreateOrUpdate(this ICache cache, UserObjectInternalDTO dto)
            => dto == null ? null : cache.Users.CreateOrUpdate<StreamUser, UserObjectInternalDTO>(dto, out _);

        public static StreamUser TryCreateOrUpdate(this ICache cache, UserObjectInternalDTO dto,
            out bool wasCreated)
        {
            wasCreated = false;
            return dto == null
                ? null
                : cache.Users.CreateOrUpdate<StreamUser, UserObjectInternalDTO>(dto, out wasCreated);
        }

        public static StreamLocalUserData TryCreateOrUpdate(this ICache cache, OwnUserInternalDTO dto)
            => dto == null ? null : cache.LocalUser.CreateOrUpdate<StreamLocalUserData, OwnUserInternalDTO>(dto, out _);
        
        public static StreamUser TryCreateOrUpdate(this ICache cache, FullUserResponseInternalDTO dto)
            => dto == null ? null : cache.Users.CreateOrUpdate4<StreamUser, FullUserResponseInternalDTO>(dto, out _);
        
        public static StreamUser TryCreateOrUpdate(this ICache cache, UserEventPayloadInternalDTO dto)
            => dto == null ? null : cache.Users.CreateOrUpdate5<StreamUser, UserEventPayloadInternalDTO>(dto, out _);
        
        public static StreamPoll TryCreateOrUpdate(this ICache cache, PollResponseDataInternalDTO dto)
            => dto == null ? null : cache.Polls.CreateOrUpdate<StreamPoll, PollResponseDataInternalDTO>(dto, out _);

        public static StreamThread TryCreateOrUpdate(this ICache cache, ThreadStateResponseInternalDTO dto)
            => dto == null ? null : cache.Threads.CreateOrUpdate<StreamThread, ThreadStateResponseInternalDTO>(dto, out _);

        public static StreamThread TryCreateOrUpdate(this ICache cache, ThreadResponseInternalDTO dto)
            => dto == null ? null : cache.Threads.CreateOrUpdate2<StreamThread, ThreadResponseInternalDTO>(dto, out _);

        public static StreamThread TryCreateOrUpdate(this ICache cache, ThreadStateInternalDTO dto)
            => dto == null ? null : cache.Threads.CreateOrUpdate3<StreamThread, ThreadStateInternalDTO>(dto, out _);
    }
}