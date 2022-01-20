using System.Collections;
using System.Collections.Generic;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.Core.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.Unity.Scripts
{
    /// <summary>
    /// Channel message list view
    /// </summary>
    public class MessageListView : BaseView
    {
        protected override void OnInited()
        {
            base.OnInited();

            State.ActiveChanelChanged += OnActiveChannelChanged;
        }

        protected override void OnDisposing()
        {
            if (_activeChannel != null)
            {
                _activeChannel.NewMessageAdded -= OnUpdated;
                _activeChannel = null;
            }

            State.ActiveChanelChanged -= OnActiveChannelChanged;
            ClearAll();

            base.OnDisposing();
        }

        private readonly List<MessageView> _messages = new List<MessageView>();

        [SerializeField]
        private Transform _messagesContainer;

        [SerializeField]
        private MessageView _messageViewPrefab;

        [SerializeField]
        private MessageView _localUserMessageViewPrefab;

        private ChannelState _activeChannel;

        private void OnActiveChannelChanged(ChannelState channel)
        {
            if (_activeChannel != null)
            {
                _activeChannel.NewMessageAdded -= OnUpdated;
                _activeChannel = null;
            }

            if (channel == null)
            {
                return;
            }

            RebuildMessages(channel);

            _activeChannel = channel;
            channel.NewMessageAdded += OnUpdated;
        }

        private void OnUpdated(ChannelState channel, Message message)
            => RebuildMessages(channel);

        private void ClearAll()
        {
            foreach (var m in _messages)
            {
                Destroy(m.gameObject);
            }

            _messages.Clear();
        }

        private void RebuildMessages(ChannelState channel)
        {
            ClearAll();

            var imageLoader = new UnityImageWebLoader();
            foreach (var message in channel.Messages)
            {
                var messageView = CreateMessageView(message);
                messageView.UpdateData(message, imageLoader);
                _messages.Add(messageView);
            }

            StartCoroutine(ScrollToBottomAfterResized());
        }

        //Todo: extract to ViewFactory
        private MessageView CreateMessageView(Message message)
        {
            var isLocal = Client.IsLocalUser(message.User);
            var prefab = isLocal ? _localUserMessageViewPrefab : _messageViewPrefab;
            var view = Instantiate(prefab, _messagesContainer);
            view.Init(ViewContext);
            return view;
        }

        private IEnumerator ScrollToBottomAfterResized()
        {
            //wait for renderer to update
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        }
    }
}