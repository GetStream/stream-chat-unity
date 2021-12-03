using System.Collections;
using System.Collections.Generic;
using Plugins.GetStreamIO.Core.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Chat view with messages
    /// </summary>
    public class ChatView : BaseView
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

        //Todo: we should append last msg, not rebuild whole view
        private void RebuildMessages(Channel channel)
        {
            ClearAll();

            foreach (var m in channel.Messages)
            {
                var messageView = Instantiate(_messageViewPrefab, _messagesContainer);
                messageView.Init(m);
                _messages.Add(messageView);
            }

            //Todo: move this dependency elsewhere - we need to wait for render resize before we scroll the view
            StartCoroutine(ScrollToBottomAfterResized());
        }

        IEnumerator ScrollToBottomAfterResized()
        {
            yield return new WaitForEndOfFrame();
            GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        }
    }
}