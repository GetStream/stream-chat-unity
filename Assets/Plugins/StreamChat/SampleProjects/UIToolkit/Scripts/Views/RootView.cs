using System;
using StreamChat.SampleProjects.UIToolkit.Config;
using StreamChat.SampleProjects.UIToolkit.Utils;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Views
{
    /// <summary>
    /// The main container
    /// </summary>
    public class RootView : BaseView<VisualElement>
    {
        public RootView(IChatState chatState, IChatWriter chatWriter, VisualElement visualElement,
            IViewFactory viewFactory, IViewConfig config)
            : base(visualElement, viewFactory, config)
        {
            _chatState = chatState ?? throw new ArgumentNullException(nameof(chatState));

            var channelsScrollView = VisualElement.Q<ScrollView>("channels-list");
            _channelsListView = viewFactory.CreateChannelsListView(_chatState, channelsScrollView);

            var channelMessagesScrollView = VisualElement.Q<ScrollView>("channel-messages");
            _channelMessagesView = viewFactory.CreateChannelMessagesView(_chatState, channelMessagesScrollView);

            var newMessageForm = VisualElement.Q<VisualElement>("new-message-form");
            _messageInputFormView = viewFactory.CreateMessageInputFormView(chatWriter, newMessageForm);
        }

        protected override void OnDispose()
        {
            _messageInputFormView?.Dispose();
            _messageInputFormView = null;

            _channelsListView?.Dispose();
            _channelsListView = null;

            _channelMessagesView?.Dispose();
            _channelMessagesView = null;

            base.OnDispose();
        }

        private readonly IChatState _chatState;

        private MessageInputFormView _messageInputFormView;
        private ChannelsListView _channelsListView;
        private ChannelMessagesView _channelMessagesView;
    }
}