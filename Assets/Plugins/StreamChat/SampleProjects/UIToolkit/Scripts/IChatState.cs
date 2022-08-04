using System;
using System.Collections.Generic;
using StreamChat.Core.Models;

namespace StreamChat.SampleProjects.UIToolkit
{
    public interface IChatState
    {
        event Action ChannelsUpdated;
        event Action<ChannelState> ActiveChanelChanged;
        OwnUser OwnUser { get; }
        IReadOnlyList<ChannelState> Channels { get; }
        ChannelState ActiveChannel { get; }
        event Action OwnUserUpdated;
    }
}