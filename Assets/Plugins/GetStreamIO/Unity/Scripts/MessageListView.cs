using System.Collections;
using System.Collections.Generic;
using Plugins.GetStreamIO.Core;
using Plugins.GetStreamIO.Core.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Channel message list view
    /// </summary>
    public class MessageListView : BaseView
    {
        protected override void OnInited()
        {
            base.OnInited();

            Client.ActiveChanelChanged += OnActiveChannelChanged;
        }

        protected override void OnDisposing()
        {
            Client.ActiveChanelChanged -= OnActiveChannelChanged;
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

        private Channel _activeChannel;

        private void OnActiveChannelChanged(Channel channel)
        {
            if (_activeChannel != null)
            {
                _activeChannel.Updated -= OnUpdated;
                _activeChannel = null;
            }

            if (channel == null)
            {
                return;
            }

            RebuildMessages(channel);

            _activeChannel = channel;
            channel.Updated += OnUpdated;
        }

        private void OnUpdated(Channel channel)
            => RebuildMessages(channel);

        private void ClearAll()
        {
            foreach (var m in _messages)
            {
                Destroy(m.gameObject);
            }

            _messages.Clear();
        }

        private void RebuildMessages(Channel channel)
        {
            ClearAll();

            var imageLoader = new UnityImageWebLoader();
            foreach (var message in channel.Messages)
            {
                var messageView = CreateMessageView(message);
                messageView.Init(message, imageLoader);
                _messages.Add(messageView);
            }

            StartCoroutine(ScrollToBottomAfterResized());
        }

        //Todo: extract to ViewFactory
        private MessageView CreateMessageView(Message message)
        {
            var isLocal = Client.IsLocalUser(message.User);
            var prefab = isLocal ? _localUserMessageViewPrefab : _messageViewPrefab;
            return Instantiate(prefab, _messagesContainer);
        }

        private IEnumerator ScrollToBottomAfterResized()
        {
            //wait 1 frame for renderer to update
            yield return new WaitForEndOfFrame();
            GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        }
    }
}