using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State.TrackedObjects;

namespace StreamChat.Core.State
{
    internal class Cache : ICache
    {
        public Cache()
        {
            Channels = new Repository<StreamChannel>(StreamChannel.Create, cache: this);
            Messages = new Repository<StreamMessage>(StreamMessage.Create, cache: this);
            Users = new Repository<StreamUser>(StreamUser.Create, cache: this);
            LocalUser = new Repository<StreamLocalUser>(StreamLocalUser.Create, cache: this);
            ChannelMembers = new Repository<StreamChannelMember>(StreamChannelMember.Create, cache: this);

            Channels.RegisterDtoTrackingIdGetter<StreamChannel, ChannelStateResponseInternalDTO>(dto => dto.Channel.Cid);

            Users.RegisterDtoTrackingIdGetter<StreamUser, UserObjectInternalInternalDTO>(dto => dto.Id);

            LocalUser.RegisterDtoTrackingIdGetter<StreamLocalUser, OwnUserInternalDTO>(dto => dto.Id);

            ChannelMembers.RegisterDtoTrackingIdGetter<StreamChannelMember, ChannelMemberInternalDTO>(dto => dto.UserId);
        }

        public IRepository<StreamChannel> Channels { get; }

        public IRepository<StreamMessage> Messages { get; }

        public IRepository<StreamUser> Users { get; }

        public IRepository<StreamLocalUser> LocalUser { get; }

        public IRepository<StreamChannelMember> ChannelMembers { get; }
    }
}