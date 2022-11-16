using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.State.Caches
{
    internal sealed class Cache : ICache
    {
        public Cache(StreamChatClient stateClient, ILogs logs)
        {
            var trackedObjectsFactory = new TrackedObjectsFactory(stateClient, logs, this);

            Channels = new CacheRepository<StreamChannel>(trackedObjectsFactory.CreateStreamChannel, cache: this);
            Messages = new CacheRepository<StreamMessage>(trackedObjectsFactory.CreateStreamMessage, cache: this);
            Users = new CacheRepository<StreamUser>(trackedObjectsFactory.CreateStreamUser, cache: this);
            LocalUser = new CacheRepository<StreamLocalUserData>(trackedObjectsFactory.CreateStreamLocalUser, cache: this);
            ChannelMembers = new CacheRepository<StreamChannelMember>(trackedObjectsFactory.CreateStreamChannelMember, cache: this);

            Channels.RegisterDtoTrackingIdGetter<StreamChannel, ChannelStateResponseInternalDTO>(dto => dto.Channel.Cid);
            Channels.RegisterDtoTrackingIdGetter<StreamChannel, ChannelResponseInternalDTO>(dto => dto.Cid);
            Channels.RegisterDtoTrackingIdGetter<StreamChannel, ChannelStateResponseFieldsInternalDTO>(dto => dto.Channel.Cid);
            Channels.RegisterDtoTrackingIdGetter<StreamChannel, UpdateChannelResponseInternalDTO>(dto => dto.Channel.Cid);

            Users.RegisterDtoTrackingIdGetter<StreamUser, UserObjectInternalInternalDTO>(dto => dto.Id);
            Users.RegisterDtoTrackingIdGetter<StreamUser, UserResponseInternalDTO>(dto => dto.Id);
            Users.RegisterDtoTrackingIdGetter<StreamUser, OwnUserInternalDTO>(dto => dto.Id);

            LocalUser.RegisterDtoTrackingIdGetter<StreamLocalUserData, OwnUserInternalDTO>(dto => dto.Id);

            //In some cases the ChannelMemberInternalDTO.UserId was null
            ChannelMembers.RegisterDtoTrackingIdGetter<StreamChannelMember, ChannelMemberInternalDTO>(dto => dto.User.Id);

            Messages.RegisterDtoTrackingIdGetter<StreamMessage, MessageInternalDTO>(dto => dto.Id);
        }

        public ICacheRepository<StreamChannel> Channels { get; }

        public ICacheRepository<StreamMessage> Messages { get; }

        public ICacheRepository<StreamUser> Users { get; }

        public ICacheRepository<StreamLocalUserData> LocalUser { get; }

        public ICacheRepository<StreamChannelMember> ChannelMembers { get; }
    }
}