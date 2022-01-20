using System;
using Plugins.GetStreamIO.Core.API;
using Plugins.GetStreamIO.Core.Auth;
using Plugins.GetStreamIO.Core.Events;
using Plugins.GetStreamIO.Core.Models;
using Action = System.Action;

namespace Plugins.GetStreamIO.Core
{
    /// <summary>
    /// GetStream.io main client
    /// </summary>
    public interface IGetStreamChatClient : IAuthProvider, IConnectionProvider, IDisposable
    {
        event Action Connected;
        event Action<string> EventReceived;
        event Action<EventMessageNew> MessageReceived;
        event Action<EventMessageDeleted> MessageDeleted;
        event Action<EventMessageUpdated> MessageUpdated;

        ConnectionState ConnectionState { get; }
        IChannelApi ChannelApi { get; }
        IMessageApi MessageApi { get; }
        IModerationApi ModerationApi { get; }

        void Update(float deltaTime);

        void Connect();

        bool IsLocalUser(User user);

        bool IsLocalUser(ChannelMember channelMember);
    }
}