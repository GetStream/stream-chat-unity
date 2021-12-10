using System;
using System.Collections.Generic;
using Plugins.GetStreamIO.Core.Auth;
using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core
{
    /// <summary>
    /// GetStream.io main client
    /// </summary>
    public interface IGetStreamChatClient : IAuthProvider, IConnectionProvider, IDisposable
    {
        event Action ChannelsUpdated;

        event Action<Channel> ActiveChanelChanged;

        IReadOnlyList<Channel> Channels { get; }
        ConnectionState ConnectionState { get; }

        void Update(float deltaTime);

        void Start();

        void OpenChannel(Channel channel);

        void SendMessage(string text);

        event Action<string> EventReceived;

        bool IsLocalUser(User user);

        bool IsLocalUser(Member member);
    }
}