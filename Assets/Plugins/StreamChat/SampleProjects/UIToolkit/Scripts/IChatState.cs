using System;
using System.Collections.Generic;
using StreamChat.Core.Models;

namespace StreamChat.SampleProjects.UIToolkit
{
    public interface IChatState
    {
        event Action ChannelsUpdated;
        event Action ActiveChannelChanged;
        event Action OwnUserUpdated;

        OwnUser OwnUser { get; }
        IReadOnlyList<ChannelState> Channels { get; }
        ChannelState ActiveChannel { get; }

        void SelectChannel(ChannelState channelState);

        event Action<Message> MessageReceived;
        event Action<Message> MessageDeleted;
        event Action<Message> MessageUpdated;
    }
}