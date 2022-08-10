using System;
using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.Models;
using StreamChat.SampleProjects.UIToolkit.Config;
using StreamChat.SampleProjects.UIToolkit.Utils;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Views
{
    /// <summary>
    /// Represents messages list of the active channel
    /// </summary>
    public class ChannelMessagesView : BaseView<ScrollView>
    {
        public ChannelMessagesView(IChatState chatState, ScrollView visualElement, IViewFactory viewFactory, IViewConfig config)
            : base(visualElement, viewFactory, config)
        {
            _chatState = chatState ?? throw new ArgumentNullException(nameof(chatState));

            _chatState.ActiveChannelChanged += OnActiveChannelChanged;
            _chatState.MessageReceived += OnMessageReceived;
            _chatState.MessageDeleted += OnMessageDeleted;
            _chatState.MessageUpdated += OnMessageUpdated;
        }

        protected override void OnDispose()
        {
            _chatState.ActiveChannelChanged -= OnActiveChannelChanged;
            _chatState.MessageReceived -= OnMessageReceived;
            _chatState.MessageDeleted -= OnMessageDeleted;
            _chatState.MessageUpdated -= OnMessageUpdated;

            base.OnDispose();
        }

        private readonly IChatState _chatState;
        private readonly List<MessageItemView> _messageViews = new List<MessageItemView>();

        private string _currentChannelCid;

        private void OnMessageUpdated(Message message)
        {
            if (message.Cid != _currentChannelCid)
            {
                return;
            }

            var messageItemView = _messageViews.FirstOrDefault(_ => _.Data.Id == message.Id);

            messageItemView?.SetData(message);
        }

        private void OnMessageDeleted(Message message)
        {
            if (message.Cid != _currentChannelCid)
            {
                return;
            }

            for (int i = _messageViews.Count - 1; i >= 0; i--)
            {
                var messageItemView = _messageViews[i];
                if (messageItemView.Data.Id != message.Id)
                {
                    continue;
                }

                _messageViews.RemoveAt(i);
                VisualElement.Remove(messageItemView.VisualElement);
            }
        }

        private void OnMessageReceived(Message message)
        {
            if (message.Cid != _currentChannelCid)
            {
                return;
            }

            AddMessage(message);
        }

        private void OnActiveChannelChanged()
        {
            ClearMessages();

            foreach (var message in _chatState.ActiveChannel.Messages)
            {
                AddMessage(message);
            }

            _currentChannelCid = _chatState.ActiveChannel.Channel.Cid;
        }

        private void AddMessage(Message message)
        {
            var messageItemView = Factory.CreateMessageItemView(message);
            VisualElement.Add(messageItemView.VisualElement);
            _messageViews.Add(messageItemView);
        }

        private void ClearMessages()
        {
            _messageViews.Clear();
            VisualElement.RemoveAllChildren();
        }
    }
}