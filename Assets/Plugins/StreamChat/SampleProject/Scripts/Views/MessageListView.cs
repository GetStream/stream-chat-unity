﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            _scrollRect = GetComponent<ScrollRect>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (_scrollRect.verticalNormalizedPosition >= 1f && !IsScrollListRebuilding)
            {
                TryLoadPreviousMessagesAsync().LogIfFailed();
            }
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

        private ScrollRect _scrollRect;

        private int _scrollListLastUpdateFrame;
        private Task _loadPreviousMessagesTask;
        private string _lastChannelCId;

        //we wait 2 frames before depending on scroll list position in order for the list to render and update its internal state
        private bool IsScrollListRebuilding => _scrollListLastUpdateFrame + 2 > Time.frameCount;

        private void OnActiveChannelChanged(ChannelState channel)
        {
            if (channel == null)
            {
                ClearAll();
                return;
            }

            var channelChanged = _lastChannelCId != channel.Channel.Cid;

            _lastChannelCId = channel.Channel.Cid;

            RebuildMessages(channel, scrollToBottom: channelChanged);
        }

        private void OnActiveChanelMessageReceived(ChannelState channel, Message message)
            => RebuildMessages(channel, scrollToBottom: false);

        private void ClearAll()
        {
            foreach (var m in _messages)
            {
                Destroy(m.gameObject);
            }

            _messages.Clear();
        }

        private void RebuildMessages(ChannelState channel, bool scrollToBottom)
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

            _scrollListLastUpdateFrame = Time.frameCount;

            if (scrollToBottom)
            {
                StartCoroutine(ScrollToBottomAfterResized());
            }
        }

        private async Task TryLoadPreviousMessagesAsync()
        {
            if (!_loadPreviousMessagesTask?.IsCompleted ?? false)
            {
                return;
            }

            var lastTopMessageId = State.ActiveChannel?.Messages.FirstOrDefault()?.Id;

            _loadPreviousMessagesTask = State.LoadPreviousMessagesAsync();

            await _loadPreviousMessagesTask;

            await Task.Delay(1); //wait 1 frame for the scroll rect render to update

            if (lastTopMessageId == null)
            {
                return;
            }

            TryScrollToPreviouslyTopMessage(lastTopMessageId);
        }

        private void TryScrollToPreviouslyTopMessage(string lastTopMessageId)
        {
            var currentTopMessageId = State.ActiveChannel.Messages.FirstOrDefault()?.Id;

            if (currentTopMessageId == lastTopMessageId)
            {
                return;
            }

            var lastTopMessage = _messages.FirstOrDefault(_ => _.Message.Id == lastTopMessageId);

            if (lastTopMessage == null)
            {
                return;
            }

            _scrollRect.content.localPosition =
                GetSnapToPositionToBringChildIntoView(_scrollRect, (RectTransform)lastTopMessage.transform);
        }

        private static Vector2 GetSnapToPositionToBringChildIntoView(ScrollRect instance, RectTransform child)
        {
            Canvas.ForceUpdateCanvases();
            var viewportLocalPosition = instance.viewport.localPosition;
            var childLocalPosition = child.localPosition;
            var result = new Vector2(
                0 - (viewportLocalPosition.x + childLocalPosition.x),
                0 - (viewportLocalPosition.y + childLocalPosition.y)
            );
            return result;
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