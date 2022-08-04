using System;
using System.Linq;
using StreamChat.Core.Models;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit
{
    public class ChannelItemView : BaseView
    {
        public ChannelItemView(VisualElement visualElement, IViewFactory viewFactory, IViewConfig config)
            : base(visualElement, viewFactory, config)
        {
            _name = VisualElement.Q<Label>("name");
            _lastMessage = VisualElement.Q<Label>("last-message");
            _lastMessageDate = VisualElement.Q<Label>("last-message-date");
        }

        public void SetData(ChannelState data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));

            _name.text = _data.Channel.Name;
            _lastMessage.text = GetLastMessageSnippet();
            _lastMessageDate.text = data.Channel.LastMessageAt?.ToString("t") ?? string.Empty;
        }

        private const int MessageSnippetLength = 20;

        private readonly Label _name;
        private readonly Label _lastMessage;
        private readonly Label _lastMessageDate;

        private ChannelState _data;

        private string GetLastMessageSnippet()
        {
            var lastMessage = _data.Messages.LastOrDefault();

            if (lastMessage == null)
            {
                return string.Empty;
            }

            var endIndex = Math.Min(lastMessage.Text.Length, MessageSnippetLength);
            return lastMessage.Text.Substring(0, endIndex);
        }
    }
}