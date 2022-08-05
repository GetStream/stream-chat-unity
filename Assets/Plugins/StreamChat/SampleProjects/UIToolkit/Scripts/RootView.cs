using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit
{
    public class RootView : BaseView
    {
        public RootView(IChatState chatState, VisualElement visualElement, IViewFactory viewFactory, IViewConfig config)
            : base(visualElement, viewFactory, config)
        {
            _chatState = chatState ?? throw new ArgumentNullException(nameof(chatState));

            _chatState.ChannelsUpdated += OnChannelsUpdated;

            _channelsScrollView = VisualElement.Q<ScrollView>("channels-list");
        }

        protected override void OnDispose()
        {
            _chatState.ChannelsUpdated -= OnChannelsUpdated;

            for (int i = _channels.Count - 1; i >= 0; i--)
            {
                _channels[i].Selected -= OnChannelSelected;
                _channels.RemoveAt(i);
            }

            base.OnDispose();
        }

        private const string ChannelItemSelectedClass = "channel-item__selected";

        private readonly List<ChannelItemView> _channels = new List<ChannelItemView>();
        private readonly IChatState _chatState;

        private readonly ScrollView _channelsScrollView;

        private void OnChannelsUpdated()
        {
            for (int i = _channelsScrollView.childCount - 1; i >= 0; i--)
            {
                _channelsScrollView.RemoveAt(i);
            }

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
            Debug.Log("Selected channel: " + channelState.Channel.Id);

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
    }
}