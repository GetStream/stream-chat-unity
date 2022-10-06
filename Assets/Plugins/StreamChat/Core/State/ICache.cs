using StreamChat.Core.State.Models;

namespace StreamChat.Core.State
{
    internal interface ICache
    {
        IRepository<StreamChannel> Channels { get; }
        IRepository<StreamMessage> Messages { get; }
        IRepository<StreamUser> Users { get; }
        IRepository<StreamLocalUser> LocalUser { get; }
        IRepository<StreamChannelMember> ChannelMembers { get; }
    }
}