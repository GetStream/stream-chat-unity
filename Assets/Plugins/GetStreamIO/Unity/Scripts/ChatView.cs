using System.Collections.Generic;
using Plugins.GetStreamIO.Core.Models;
using UnityEngine;

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
            DestroyAll();

            base.OnDisposing();
        }

        private readonly List<MessageView> _messages = new List<MessageView>();

        [SerializeField]
        private Transform _messagesContainer;

        [SerializeField]
        private MessageView _messageViewPrefab;

        private void OnActiveChannelChanged(Channel channel)
        {
            if (channel == null)
            {
                return;
            }

            DestroyAll();

            foreach (var m in channel.Messages)
            {
                var messageView = Instantiate(_messageViewPrefab, _messagesContainer);
                messageView.Init(m);
                _messages.Add(messageView);
            }
        }

        private void DestroyAll()
        {
            foreach (var m in _messages)
            {
                Destroy(m.gameObject);
            }

            _messages.Clear();
        }
    }
}