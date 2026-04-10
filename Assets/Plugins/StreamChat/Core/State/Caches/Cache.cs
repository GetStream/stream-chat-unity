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
            Polls = new CacheRepository<StreamPoll>(trackedObjectsFactory.CreateStreamPoll, cache: this);

            Channels.RegisterDtoIdMapping<StreamChannel, ChannelStateResponseInternalDTO>(dto => dto.Channel.Cid);
            Channels.RegisterDtoIdMapping2<StreamChannel, ChannelResponseInternalDTO>(dto => dto.Cid);
            Channels.RegisterDtoIdMapping3<StreamChannel, ChannelStateResponseFieldsInternalDTO>(dto => dto.Channel.Cid);
            Channels.RegisterDtoIdMapping4<StreamChannel, UpdateChannelResponseInternalDTO>(dto => dto.Channel.Cid);

            Users.RegisterDtoIdMapping<StreamUser, UserObjectInternalDTO>(dto => dto.Id);
            Users.RegisterDtoIdMapping2<StreamUser, UserResponseInternalDTO>(dto => dto.Id);
            Users.RegisterDtoIdMapping3<StreamUser, OwnUserInternalDTO>(dto => dto.Id);
            Users.RegisterDtoIdMapping4<StreamUser, FullUserResponseInternalDTO>(dto => dto.Id);
            Users.RegisterDtoIdMapping5<StreamUser, UserEventPayloadInternalDTO>(dto => dto.Id);

            LocalUser.RegisterDtoIdMapping<StreamLocalUserData, OwnUserInternalDTO>(dto => dto.Id);

            //In some cases the ChannelMemberInternalDTO.UserId was null -> only known case is channelDto.Membership
            ChannelMembers.RegisterDtoIdMapping<StreamChannelMember, ChannelMemberInternalDTO>(dto =>
            {
                if(dto.User != null)
                {
                    return dto.User.Id;
                }

                return dto.UserId;
            });

            Messages.RegisterDtoIdMapping<StreamMessage, MessageInternalDTO>(dto => dto.Id);
            Messages.RegisterDtoIdMapping2<StreamMessage, MessageResponseInternalDTO>(dto => dto.Id);

            Polls.RegisterDtoIdMapping<StreamPoll, PollResponseDataInternalDTO>(dto => dto.Id);
        }

        public ICacheRepository<StreamChannel> Channels { get; }

        public ICacheRepository<StreamMessage> Messages { get; }

        public ICacheRepository<StreamUser> Users { get; }

        public ICacheRepository<StreamLocalUserData> LocalUser { get; }

        public ICacheRepository<StreamChannelMember> ChannelMembers { get; }

        public ICacheRepository<StreamPoll> Polls { get; }
    }
}