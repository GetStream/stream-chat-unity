using System;
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

            _channelsList = VisualElement.Q<ScrollView>("channels-list");
        }

        protected override void OnDispose()
        {
            _chatState.ChannelsUpdated -= OnChannelsUpdated;

            base.OnDispose();
        }

        private readonly IChatState _chatState;

        private readonly ScrollView _channelsList;

        private void OnChannelsUpdated()
        {
            for (int i = _channelsList.childCount - 1; i >= 0; i--)
            {
                _channelsList.RemoveAt(i);
            }

            foreach (var channel in _chatState.Channels)
            {
                var channelItemView = Factory.CreateChannelItemView(channel);
                _channelsList.Add(channelItemView.VisualElement);
            }
        }

        private void FillChannelsList()
        {

        }
    }
}