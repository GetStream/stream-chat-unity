using StreamChat.Core.Models;
using StreamChat.SampleProjects.UIToolkit.Views;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit
{
    public interface IViewFactory
    {
        RootView CreateRootView(IChatState chatState, IChatWriter chatWriter);

        ChannelItemView CreateChannelItemView(ChannelState channelState);

        MessageItemView CreateMessageItemView(Message message);

        MessageInputFormView CreateMessageInputFormView(IChatWriter chatWriter,
            VisualElement instance = null);

        ChannelsListView CreateChannelsListView(IChatState chatState, ScrollView scrollView);

        ChannelMessagesView CreateChannelMessagesView(IChatState chatState, ScrollView scrollView);
    }
}