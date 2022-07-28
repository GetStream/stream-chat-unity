using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.Models;
using StreamChat.SampleProject.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Views
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
            State.ActiveChanelMessageReceived += OnActiveChanelMessageReceived;
        }

        protected override void OnDisposing()
        {
            State.ActiveChanelChanged -= OnActiveChannelChanged;
            State.ActiveChanelMessageReceived -= OnActiveChanelMessageReceived;

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

        private void OnActiveChannelChanged(ChannelState channel)
        {
            if (channel == null)
            {
                ClearAll();
                return;
            }

            RebuildMessages(channel);
        }

        private void OnActiveChanelMessageReceived(ChannelState channel, Message message)
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

                if (message == channel.Messages.Last())
                {
                    messageView.TryPlay();
                }
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