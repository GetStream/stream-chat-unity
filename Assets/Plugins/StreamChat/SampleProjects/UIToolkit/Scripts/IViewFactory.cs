using StreamChat.Core.Models;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit
{
    public interface IViewFactory
    {
        RootView CreateRootView(IChatState chatState);

        ChannelItemView CreateChannelItemView(ChannelState channelState);
    }
}