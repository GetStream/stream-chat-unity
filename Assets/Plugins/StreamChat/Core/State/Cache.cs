using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.State
{
    internal class Cache : ICache
    {
        public Cache(StreamChatStateClient stateClient, ILogs logs)
        {
            var trackedObjectsFactory = new TrackedObjectsFactory(stateClient, logs, this);

            Channels = new Repository<StreamChannel>((uniqueId, repository) => trackedObjectsFactory.CreateStreamChannel(uniqueId), cache: this);
            Messages = new Repository<StreamMessage>((uniqueId, repository) => trackedObjectsFactory.CreateStreamMessage(uniqueId), cache: this);
            Users = new Repository<StreamUser>((uniqueId, repository) => trackedObjectsFactory.CreateStreamUser(uniqueId), cache: this);
            LocalUser = new Repository<StreamLocalUser>((uniqueId, repository) => trackedObjectsFactory.CreateStreamLocalUser(uniqueId), cache: this);
            ChannelMembers = new Repository<StreamChannelMember>((uniqueId, repository) => trackedObjectsFactory.CreateStreamChannelMember(uniqueId), cache: this);

            Channels.RegisterDtoTrackingIdGetter<StreamChannel, ChannelStateResponseInternalDTO>(dto => dto.Channel.Cid);
            Channels.RegisterDtoTrackingIdGetter<StreamChannel, ChannelResponseInternalDTO>(dto => dto.Cid);
            Channels.RegisterDtoTrackingIdGetter<StreamChannel, ChannelStateResponseFieldsInternalDTO>(dto => dto.Channel.Cid);

            Users.RegisterDtoTrackingIdGetter<StreamUser, UserObjectInternalInternalDTO>(dto => dto.Id);
            Users.RegisterDtoTrackingIdGetter<StreamUser, UserResponseInternalDTO>(dto => dto.Id);
            Users.RegisterDtoTrackingIdGetter<StreamUser, OwnUserInternalDTO>(dto => dto.Id);

            LocalUser.RegisterDtoTrackingIdGetter<StreamLocalUser, OwnUserInternalDTO>(dto => dto.Id);

            ChannelMembers.RegisterDtoTrackingIdGetter<StreamChannelMember, ChannelMemberInternalDTO>(dto => dto.UserId);

            Messages.RegisterDtoTrackingIdGetter<StreamMessage, MessageInternalDTO>(dto => dto.Id);
        }

        public IRepository<StreamChannel> Channels { get; }

        public IRepository<StreamMessage> Messages { get; }

        public IRepository<StreamUser> Users { get; }

        public IRepository<StreamLocalUser> LocalUser { get; }

        public IRepository<StreamChannelMember> ChannelMembers { get; }
    }
}