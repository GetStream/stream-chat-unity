using StreamChat.Core.Models;
using StreamChat.SampleProjects.UIToolkit.Views;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit
{
    public interface IViewFactory
    {
        RootView CreateRootView(IChatState chatState);

        ChannelItemView CreateChannelItemView(ChannelState channelState);

        MessageItemView CreateMessageItemView(Message message);

        MessageInputFormView CreateMessageInputFormView(IChatWriter chatWriter);
    }
}