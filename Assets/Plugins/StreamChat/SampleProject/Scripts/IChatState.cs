using System;
using System.Collections.Generic;
using StreamChat.Core.Models;

namespace StreamChat.SampleProject
{
    /// <summary>
    /// Keep chat state
    /// </summary>
    public interface IChatState : IDisposable
    {
        event Action<ChannelState> ActiveChanelChanged;
        event Action ChannelsUpdated;
        event Action<Message> MessageEditRequested;

        ChannelState ActiveChannel { get; }
        IReadOnlyList<ChannelState> Channels { get; }


        void OpenChannel(ChannelState channel);

        void EditMessage(Message message);
    }
}