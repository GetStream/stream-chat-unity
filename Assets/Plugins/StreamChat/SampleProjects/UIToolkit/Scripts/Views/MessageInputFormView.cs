using StreamChat.SampleProjects.UIToolkit.Config;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Views
{
    public class MessageInputFormView : BaseView
    {
        public MessageInputFormView(VisualElement visualElement, IViewFactory viewFactory, IViewConfig config)
            : base(visualElement, viewFactory, config)
        {
            _messageInput = VisualElement.Q<TextField>("message-input");
            _emojiPickerButton = VisualElement.Q<Button>("emoji-picker-button");
            _sendButton = VisualElement.Q<Button>("send-message-button");
        }

        private readonly TextField _messageInput;
        private readonly Button _emojiPickerButton;
        private readonly Button _sendButton;
    }
}