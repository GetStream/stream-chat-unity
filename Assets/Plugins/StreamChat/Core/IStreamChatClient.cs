using System;
using StreamChat.Core.API;
using StreamChat.Core.Auth;
using StreamChat.Core.Events;
using StreamChat.Core.Models;

namespace StreamChat.Core
{
    /// <summary>
    /// Stream Chat Client
    /// </summary>
    public interface IStreamChatClient : IAuthProvider, IConnectionProvider, IDisposable
    {
        event Action<string> EventReceived;
        event Action<EventMessageNew> MessageReceived;
        event Action<EventMessageDeleted> MessageDeleted;
        event Action<EventMessageUpdated> MessageUpdated;

        event Action<EventReactionNew> ReactionReceived;
        event Action<EventReactionUpdated> ReactionUpdated;
        event Action<EventReactionDeleted> ReactionDeleted;

        ConnectionState ConnectionState { get; }
        IChannelApi ChannelApi { get; }
        IMessageApi MessageApi { get; }
        IModerationApi ModerationApi { get; }
        IUserApi UserApi { get; }

        void Update(float deltaTime);

        void Connect();

        bool IsLocalUser(User user);

        bool IsLocalUser(ChannelMember channelMember);
    }
}