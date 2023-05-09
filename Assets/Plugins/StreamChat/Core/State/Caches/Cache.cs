using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.State.Caches
{
    internal sealed class Cache : ICache
    {
        public Cache(StreamChatClient stateClient, ISerializer serializer, ILogs logs)
        {
            var trackedObjectsFactory = new StatefulModelsFactory(stateClient, serializer, logs, this);

            Channels = new CacheRepository<StreamChannel>(trackedObjectsFactory.CreateStreamChannel, cache: this);
            Messages = new CacheRepository<StreamMessage>(trackedObjectsFactory.CreateStreamMessage, cache: this);
            Users = new CacheRepository<StreamUser>(trackedObjectsFactory.CreateStreamUser, cache: this);
            LocalUser = new CacheRepository<StreamLocalUserData>(trackedObjectsFactory.CreateStreamLocalUser, cache: this);
            ChannelMembers = new CacheRepository<StreamChannelMember>(trackedObjectsFactory.CreateStreamChannelMember, cache: this);

            Channels.RegisterDtoIdMapping<StreamChannel, ChannelStateResponseInternalDTO>(dto => dto.Channel.Cid);
            Channels.RegisterDtoIdMapping<StreamChannel, ChannelResponseInternalDTO>(dto => dto.Cid);
            Channels.RegisterDtoIdMapping<StreamChannel, ChannelStateResponseFieldsInternalDTO>(dto => dto.Channel.Cid);
            Channels.RegisterDtoIdMapping<StreamChannel, UpdateChannelResponseInternalDTO>(dto => dto.Channel.Cid);

            Users.RegisterDtoIdMapping<StreamUser, UserObjectInternalDTO>(dto => dto.Id);
            Users.RegisterDtoIdMapping<StreamUser, UserResponseInternalDTO>(dto => dto.Id);
            Users.RegisterDtoIdMapping<StreamUser, OwnUserInternalDTO>(dto => dto.Id);

            LocalUser.RegisterDtoIdMapping<StreamLocalUserData, OwnUserInternalDTO>(dto => dto.Id);

            //In some cases the ChannelMemberInternalDTO.UserId was null
            ChannelMembers.RegisterDtoIdMapping<StreamChannelMember, ChannelMemberInternalDTO>(dto => dto.User.Id);

            Messages.RegisterDtoIdMapping<StreamMessage, MessageInternalDTO>(dto => dto.Id);
        }

        public ICacheRepository<StreamChannel> Channels { get; }

        public ICacheRepository<StreamMessage> Messages { get; }

        public ICacheRepository<StreamUser> Users { get; }

        public ICacheRepository<StreamLocalUserData> LocalUser { get; }

        public ICacheRepository<StreamChannelMember> ChannelMembers { get; }
    }
}