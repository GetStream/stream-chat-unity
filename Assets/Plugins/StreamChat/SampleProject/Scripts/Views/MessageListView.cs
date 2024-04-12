using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core.Models;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Utils;
using StreamChat.SampleProject.Pooling;
using StreamChat.SampleProject.Popups;
using StreamChat.SampleProject.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

            _scrollRect = GetComponent<ScrollRect>();
            
            _otherUsersMessagesPool = new ObjectsPool<MessageView>(CreateOtherUserMessageView, DestroyMessageView);
            _localUserMessagesPool = new ObjectsPool<MessageView>(CreateLocalUserMessageView, DestroyMessageView);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            HideContextMenuIfTouchedOutside();

#endif

            if (_scrollRect.content.rect.height < _scrollRect.viewport.rect.height)
            {
                // if scroll view doesn't fill whole screen the verticalNormalizedPosition is 1 so it calls for previous messages on empty message list
                return;
            }

            // Check against 1f + threshold because idle ScrollRect after scrolling can have fractional values above 1f
            if (_scrollRect.verticalNormalizedPosition >= 1.05f && !IsScrollListRebuilding)
            {
                TryLoadPreviousMessagesAsync().LogIfFailed();
            }
        }

        protected override void OnDisposing()
        {
            State.ActiveChanelChanged -= OnActiveChannelChanged;

            ClearAll();

            base.OnDisposing();
        }

        private readonly List<MessageView> _messages = new List<MessageView>();
        private readonly UnityImageWebLoader _imageLoader = new UnityImageWebLoader();

        private ObjectsPool<MessageView> _otherUsersMessagesPool;
        private ObjectsPool<MessageView> _localUserMessagesPool;

        [SerializeField]
        private Transform _messagesContainer;

        [SerializeField]
        private MessageView _messageViewPrefab;

        [SerializeField]
        private MessageView _localUserMessageViewPrefab;

        //StreamTodo: investigate optimized alternatives for ScrollRect. The default Unity ScrollRect allocates lots of memory during updates, even without instantiating new objects
        private ScrollRect _scrollRect;

        private int _scrollListLastUpdateFrame;
        private Task _loadPreviousMessagesTask;
        private IStreamChannel _activeChannel;

        private MessageOptionsPopup _activePopup;
        private int _frameShownPopup;

        //we wait 2 frames before depending on scroll list position in order for the list to render and update its internal state
        private bool IsScrollListRebuilding => _scrollListLastUpdateFrame + 2 > Time.frameCount;

        private void OnActiveChannelChanged(IStreamChannel channel)
        {
            if (_activeChannel != null)
            {
                _activeChannel.MessageReceived -= OnMessageReceived;
                _activeChannel.MessageDeleted -= OnMessageDeleted;
                _activeChannel.MessageUpdated -= OnMessageUpdated;
                _activeChannel.ReactionAdded -= OnReactionAdded;
                _activeChannel.ReactionUpdated -= OnReactionUpdated;
                _activeChannel.ReactionRemoved -= OnReactionRemoved;
            }

            if (channel == null)
            {
                ClearAll();
                return;
            }

            _activeChannel = channel;
            _activeChannel.MessageReceived += OnMessageReceived;
            _activeChannel.MessageDeleted += OnMessageDeleted;
            _activeChannel.MessageUpdated += OnMessageUpdated;
            _activeChannel.ReactionAdded += OnReactionAdded;
            _activeChannel.ReactionUpdated += OnReactionUpdated;
            _activeChannel.ReactionRemoved += OnReactionRemoved;

            RebuildMessages(channel, scrollToBottom: true);
        }

        private void OnReactionRemoved(IStreamChannel channel, IStreamMessage message, StreamReaction reaction)
            => RebuildMessages(channel, scrollToBottom: false);

        private void OnReactionUpdated(IStreamChannel channel, IStreamMessage message, StreamReaction reaction)
            => RebuildMessages(channel, scrollToBottom: false);

        private void OnReactionAdded(IStreamChannel channel, IStreamMessage message, StreamReaction reaction)
            => RebuildMessages(channel, scrollToBottom: false);

        private void OnMessageUpdated(IStreamChannel channel, IStreamMessage message)
            => RebuildMessages(channel, scrollToBottom: false);

        private void OnMessageDeleted(IStreamChannel channel, IStreamMessage message, bool isharddelete)
            => RebuildMessages(channel, scrollToBottom: false);

        private void OnMessageReceived(IStreamChannel channel, IStreamMessage message)
            => RebuildMessages(channel, scrollToBottom: true);

        private void ClearAll()
        {
            foreach (var m in _messages)
            {
                var pool = GetMessagePool(m.Message);
                m.PointedDown -= OnMessagePointedDown;
                pool.Return(m);
            }

            _messages.Clear();
        }

        private void RebuildMessages(IStreamChannel channel, bool scrollToBottom)
        {
            ClearAll();

            foreach (var message in channel.Messages)
            {
                var messageView = CreateMessageView(message);
                messageView.UpdateData(message, _imageLoader);
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

        //StreamTodo: extract to ViewFactory
        private MessageView CreateMessageView(IStreamMessage message)
        {
            var pool = GetMessagePool(message);

            var view = pool.Rent();
            view.PointedDown += OnMessagePointedDown;
            return view;
        }

        private ObjectsPool<MessageView> GetMessagePool(IStreamMessage message)
        {
            var isLocal = Client.IsLocalUser(message.User);
            return isLocal ? _localUserMessagesPool : _otherUsersMessagesPool;
        }

        private MessageView CreateLocalUserMessageView() => CreateMessageView(_localUserMessageViewPrefab);

        private MessageView CreateOtherUserMessageView() => CreateMessageView(_messageViewPrefab);

        private MessageView CreateMessageView(MessageView prefab)
        {
            var view =  Instantiate(prefab, _messagesContainer);
            view.Init(ViewContext);
            view.gameObject.SetActive(false);
            return view;
        }

        private void DestroyMessageView(MessageView view)
        {
            view.PointedDown -= OnMessagePointedDown;
            Destroy(view.gameObject);
        }

        private IEnumerator ScrollToBottomAfterResized()
        {
            //wait for renderer to update
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        }

        private void OnMessagePointedDown(MessageView messageView, PointerEventData pointerEventData)
        {
#if UNITY_STANDALONE
            if (!InputSystem.GetMouseButton(1))
            {
                return;
            }
#endif

            ShowContextMenu(messageView, pointerEventData);
        }

        private void ShowContextMenu(MessageView parent, PointerEventData pointerEventData)
        {
            HideContextMenu();

            var pointerPosition = pointerEventData.position;

            _activePopup = Factory.CreateMessageOptionsPopup(parent, State);

            var rectTransform = ((RectTransform)_activePopup.transform);

            rectTransform.position = pointerPosition + new Vector2(-10, 10);

            _frameShownPopup = Time.frameCount;
        }

        private void HideContextMenu()
        {
            if (_activePopup != null)
            {
                Destroy(_activePopup.gameObject);
                _activePopup = null;
            }
        }

        private void HideContextMenuIfTouchedOutside()
        {
            if (_frameShownPopup == Time.frameCount)
            {
                return;
            }

            if (Input.touchCount == 0 || _activePopup == null)
            {
                return;
            }

            var anyTouchOnPopup = false;
            for (int i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);

                if (RectTransformUtility.RectangleContainsScreenPoint(_activePopup.RectTransform,
                        touch.position))
                {
                    anyTouchOnPopup = true;
                }
            }

            if (!anyTouchOnPopup)
            {
                HideContextMenu();
            }
        }
    }
}