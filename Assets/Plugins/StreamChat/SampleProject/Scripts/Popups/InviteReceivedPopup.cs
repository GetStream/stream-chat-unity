using System;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Core.StatefulModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Popups
{
    public class InviteReceivedPopup : BaseFullscreenPopup
    {
        public void SetData(IStreamChannel channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));

            var text = _textTemplate;
            text = text.Replace(ChannelNameKey, channel.Name);

            _text.text = text;
        }

        protected override void OnInited()
        {
            base.OnInited();

            _acceptButton.onClick.AddListener(OnAcceptButtonClicked);
            _rejectButton.onClick.AddListener(OnRejectButtonClicked);

            _textTemplate = _text.text;

            if (!_textTemplate.Contains(ChannelNameKey))
            {
                Debug.LogError($"The UI text template does not contain the `{ChannelNameKey}` key.");
            }
        }

        protected override void OnDisposing()
        {
            _acceptButton.onClick.RemoveListener(OnAcceptButtonClicked);
            _rejectButton.onClick.RemoveListener(OnRejectButtonClicked);

            base.OnDisposing();
        }

        private const string ChannelNameKey = "CHANNEL_NAME";

        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private Button _acceptButton;

        [SerializeField]
        private Button _rejectButton;

        private IStreamChannel _channel;
        private string _textTemplate;
        private bool _isProcessing;

        private void OnRejectButtonClicked()
        {
            if (_isProcessing)
            {
                return;
            }

            _isProcessing = true;
            var threadId = Thread.CurrentThread.ManagedThreadId;
            _channel.RejectInviteAsync().ContinueWith(t =>
            {
                _isProcessing = false;
                if (t.IsFaulted)
                {
                    Debug.LogException(t.Exception);
                    return;
                }
                
                var threadId2 = Thread.CurrentThread.ManagedThreadId;
                if (threadId != threadId2)
                {
                    Debug.LogError($"----------------------- Thread Mismatch: {threadId} vs {threadId2}");
                }

                Debug.Log($"Rejected invitation to {_channel.Name}");
                Hide();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnAcceptButtonClicked()
        {
            if (_isProcessing)
            {
                return;
            }

            _isProcessing = true;
            var threadId = Thread.CurrentThread.ManagedThreadId;
            _channel.AcceptInviteAsync().ContinueWith(t =>
            {
                _isProcessing = false;
                if (t.IsFaulted)
                {
                    Debug.LogException(t.Exception);
                    return;
                }
                
                var threadId2 = Thread.CurrentThread.ManagedThreadId;
                if (threadId != threadId2)
                {
                    Debug.LogError($"----------------------- Thread Mismatch: {threadId} vs {threadId2}");
                }

                Debug.Log($"Accepted invitation to {_channel.Name}");
                Hide();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}