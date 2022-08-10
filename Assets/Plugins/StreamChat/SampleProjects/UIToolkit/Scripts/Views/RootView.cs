using System;
using System.Collections.Generic;
using StreamChat.SampleProjects.UIToolkit.Config;
using StreamChat.SampleProjects.UIToolkit.Utils;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Views
{
    public class RootView : BaseView
    {
        public RootView(IChatState chatState, IChatWriter chatWriter, VisualElement visualElement,
            IViewFactory viewFactory, IViewConfig config)
            : base(visualElement, viewFactory, config)
        {
            _chatState = chatState ?? throw new ArgumentNullException(nameof(chatState));

            _chatState.ChannelsUpdated += OnChannelsUpdated;
            _chatState.ActiveChannelChanged += OnActiveChannelChanged;

            _channelsScrollView = VisualElement.Q<ScrollView>("channels-list");
            _channelMessagesScrollView = VisualElement.Q<ScrollView>("channel-messages");

            var newMessageForm = VisualElement.Q<VisualElement>("new-message-form");
            _messageInputFormView = viewFactory.CreateMessageInputFormView(chatWriter, newMessageForm);
        }

        protected override void OnDispose()
        {
            _chatState.ChannelsUpdated -= OnChannelsUpdated;

            for (int i = _channels.Count - 1; i >= 0; i--)
            {
                _channels[i].Selected -= OnChannelSelected;
                _channels.RemoveAt(i);
            }

            _messageInputFormView?.Dispose();
            _messageInputFormView = null;

            base.OnDispose();
        }

        private const string ChannelItemSelectedClass = "channel-item__selected";

        private readonly List<ChannelItemView> _channels = new List<ChannelItemView>();
        private readonly IChatState _chatState;

        private readonly ScrollView _channelsScrollView;
        private readonly ScrollView _channelMessagesScrollView;

        private MessageInputFormView _messageInputFormView;

        private void OnChannelsUpdated()
        {
            _channelsScrollView.RemoveAllChildren();

            foreach (var channel in _chatState.Channels)
            {
                var channelItemView = Factory.CreateChannelItemView(channel);
                _channelsScrollView.Add(channelItemView.VisualElement);
                _channels.Add(channelItemView);

                channelItemView.Selected += OnChannelSelected;
            }
        }

        private void OnChannelSelected(ChannelItemView channelItem)
        {
            var channelState = channelItem.Data;

            _chatState.SelectChannel(channelState);

            foreach (var item in _channels)
            {
                if (item == channelItem)
                {
                    item.VisualElement.AddToClassList(ChannelItemSelectedClass);
                    continue;
                }

                item.VisualElement.RemoveFromClassList(ChannelItemSelectedClass);
            }
        }

        private void OnActiveChannelChanged()
        {
            _channelMessagesScrollView.RemoveAllChildren();

            foreach (var message in _chatState.ActiveChannel.Messages)
            {
                var messageItemView = Factory.CreateMessageItemView(message);
                _channelMessagesScrollView.Add(messageItemView.VisualElement);
            }
        }
    }
}