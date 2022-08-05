using System;
using System.Collections.Generic;
using StreamChat.Core.Models;

namespace StreamChat.SampleProjects.UIToolkit
{
    public interface IChatState
    {
        event Action ChannelsUpdated;
        event Action<ChannelState> ActiveChanelChanged;
        event Action OwnUserUpdated;

        OwnUser OwnUser { get; }
        IReadOnlyList<ChannelState> Channels { get; }
        ChannelState ActiveChannel { get; }

        void SelectChannel(ChannelState channelState);
    }
}