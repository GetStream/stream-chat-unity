using System;
using System.Linq;
using StreamChat.Core.Models;
using StreamChat.SampleProjects.UIToolkit.Config;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Views
{
    /// <summary>
    /// Represents single channel preview in the channels list
    /// </summary>
    public class ChannelItemView : BaseDataView<ChannelState, VisualElement>
    {
        public event Action<ChannelItemView> Selected;

        public ChannelItemView(VisualElement visualElement, IViewFactory viewFactory, IViewConfig config)
            : base(visualElement, viewFactory, config)
        {
            _name = VisualElement.Q<Label>("name");
            _lastMessage = VisualElement.Q<Label>("last-message");
            _lastMessageDate = VisualElement.Q<Label>("last-message-date");

            visualElement.RegisterCallback<ClickEvent>(OnClickEvent);
        }

        protected override void OnDataSet(ChannelState data)
        {
            base.OnDataSet(data);

            _name.text = Data.Channel.Name;
            _lastMessage.text = GetLastMessageSnippet();
            _lastMessageDate.text = data.Channel.LastMessageAt?.ToString("t") ?? string.Empty;
        }

        protected override void OnDispose()
        {
            VisualElement.UnregisterCallback<ClickEvent>(OnClickEvent);

            base.OnDispose();
        }

        private const int MessageSnippetLength = 20;

        private readonly Label _name;
        private readonly Label _lastMessage;
        private readonly Label _lastMessageDate;

        private string GetLastMessageSnippet()
        {
            var lastMessage = Data.Messages.LastOrDefault();

            if (lastMessage == null)
            {
                return string.Empty;
            }

            var endIndex = Math.Min(lastMessage.Text.Length, MessageSnippetLength);
            return lastMessage.Text.Substring(0, endIndex);
        }

        private void OnClickEvent(ClickEvent clickEvent)
        {
            Selected?.Invoke(this);
        }
    }
}