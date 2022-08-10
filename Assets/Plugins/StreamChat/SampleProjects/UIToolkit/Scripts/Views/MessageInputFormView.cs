using System;
using System.Threading.Tasks;
using StreamChat.Libs.Utils;
using StreamChat.SampleProjects.UIToolkit.Config;
using UnityEngine;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Views
{
    public class MessageInputFormView : BaseView
    {
        public MessageInputFormView(IChatWriter chatWriter, VisualElement visualElement, IViewFactory viewFactory,
            IViewConfig config)
            : base(visualElement, viewFactory, config)
        {
            _chatWriter = chatWriter ?? throw new ArgumentNullException(nameof(chatWriter));
            _messageInput = VisualElement.Q<TextField>("message-input");
            _emojiPickerButton = VisualElement.Q<Button>("emoji-picker-button");
            _sendButton = VisualElement.Q<Button>("send-message-button");

            _sendButton.clickable.clicked += OnSendButtonClicked;

            _defaultMessageInputText = _messageInput.text;
        }

        protected override void OnDispose()
        {
            _sendButton.clickable.clicked -= OnSendButtonClicked;

            base.OnDispose();
        }

        private readonly TextField _messageInput;
        private readonly Button _emojiPickerButton;
        private readonly Button _sendButton;

        private readonly IChatWriter _chatWriter;

        private readonly string _defaultMessageInputText;

        private void OnSendButtonClicked()
        {
            if (_messageInput.text.IsNullOrEmpty())
            {
                return;
            }

            if (_defaultMessageInputText == _messageInput.text)
            {
                return;
            }

            _chatWriter.SendNewMessageAsync(_messageInput.text).ContinueWith(continuation =>
            {
                if (continuation.IsFaulted)
                {
                    Debug.LogException(continuation.Exception);
                    return;
                }

                var success = continuation.Result;
                if (success)
                {
                    _messageInput.SetValueWithoutNotify(_defaultMessageInputText);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}