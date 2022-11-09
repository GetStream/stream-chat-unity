using StreamChat.Core.State.TrackedObjects;

namespace StreamChat.Core.State
{
    internal interface ICache
    {
        ICacheRepository<StreamChannel> Channels { get; }
        ICacheRepository<StreamMessage> Messages { get; }
        ICacheRepository<StreamUser> Users { get; }
        ICacheRepository<StreamLocalUserData> LocalUser { get; }
        ICacheRepository<StreamChannelMember> ChannelMembers { get; }
    }
}