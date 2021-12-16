using System;
using System.Collections.Generic;
using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Unity
{
    /// <summary>
    /// Keep chat state
    /// </summary>
    public interface IChatState : IDisposable
    {
        event Action<Channel> ActiveChanelChanged;
        event Action ChannelsUpdated;

        Channel ActiveChannel { get; }
        IReadOnlyList<Channel> Channels { get; }


        void OpenChannel(Channel channel);
    }
}