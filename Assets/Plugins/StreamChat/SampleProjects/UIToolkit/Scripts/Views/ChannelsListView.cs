using System;
using System.Collections.Generic;
using System.Linq;
using StreamChat.SampleProjects.UIToolkit.Config;
using StreamChat.SampleProjects.UIToolkit.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Views
{
    /// <summary>
    /// Represents list of channels
    /// </summary>
    public class ChannelsListView : BaseView<ScrollView>
    {
        public ChannelsListView(IChatState chatState, ScrollView visualElement, IViewFactory viewFactory,
            IViewConfig config)
            : base(visualElement, viewFactory, config)
        {
            _chatState = chatState ?? throw new ArgumentNullException(nameof(chatState));

            _chatState.ChannelsUpdated += OnChannelsUpdated;
            _chatState.ActiveChannelChanged += OnActiveChannelChanged;
        }

        public void SelectChannel(ChannelItemView channelItemView)
        {
            var channelState = channelItemView.Data;

            _chatState.SelectChannel(channelState);

            MarkChannelSelected(channelItemView);
        }

        protected override void OnDispose()
        {
            _chatState.ChannelsUpdated -= OnChannelsUpdated;
            _chatState.ActiveChannelChanged -= OnActiveChannelChanged;

            for (int i = _channels.Count - 1; i >= 0; i--)
            {
                _channels[i].Selected -= OnChannelSelected;
                _channels.RemoveAt(i);
            }

            base.OnDispose();
        }

        private const string ChannelItemSelectedClass = "channel-item__selected";

        private readonly IChatState _chatState;
        private readonly List<ChannelItemView> _channels = new List<ChannelItemView>();

        private void OnChannelsUpdated()
        {
            VisualElement.RemoveAllChildren();

            foreach (var channel in _chatState.Channels)
            {
                var channelItemView = Factory.CreateChannelItemView(channel);
                VisualElement.Add(channelItemView.VisualElement);
                _channels.Add(channelItemView);

                channelItemView.Selected += OnChannelSelected;
            }
        }

        private void OnChannelSelected(ChannelItemView channelItemView)
            => SelectChannel(channelItemView);

        private void OnActiveChannelChanged()
        {
            if (_chatState.ActiveChannel == null)
            {
                _channels.ForEach(_ => _.VisualElement.RemoveFromClassList(ChannelItemSelectedClass));
                return;
            }

            var activeChannelCid = _chatState.ActiveChannel.Channel.Cid;
            var activeChannelItemView = _channels.FirstOrDefault(_ => _.Data.Channel.Cid == activeChannelCid);

            if (activeChannelItemView == null)
            {
                Debug.LogError($"Active channel changed to CID `{activeChannelCid}`, but failed to find item view");
                return;
            }

            MarkChannelSelected(activeChannelItemView);
        }

        private void MarkChannelSelected(ChannelItemView channelItemView)
        {
            foreach (var item in _channels)
            {
                if (item == channelItemView)
                {
                    item.VisualElement.AddToClassList(ChannelItemSelectedClass);
                    continue;
                }

                item.VisualElement.RemoveFromClassList(ChannelItemSelectedClass);
            }
        }
    }
}