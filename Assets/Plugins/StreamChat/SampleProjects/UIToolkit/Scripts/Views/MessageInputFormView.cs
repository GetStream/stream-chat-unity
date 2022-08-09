using System;
using StreamChat.Libs.Utils;
using StreamChat.SampleProjects.UIToolkit.Config;
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

        private void OnSendButtonClicked()
        {
            if (_messageInput.value.IsNullOrEmpty())
            {
                return;
            }

            _chatWriter.SendNewMessageAsync(_messageInput.value).LogOnFaulted();
        }
    }
}