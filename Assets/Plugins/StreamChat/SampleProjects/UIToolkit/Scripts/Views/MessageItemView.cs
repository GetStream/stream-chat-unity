using StreamChat.Core.Models;
using StreamChat.SampleProjects.UIToolkit.Config;
using StreamChat.SampleProjects.UIToolkit.Utils;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Views
{
    /// <summary>
    /// Represents single chat message
    /// </summary>
    public class MessageItemView : BaseDataView<Message, VisualElement>
    {
        public MessageItemView(VisualElement visualElement, IViewFactory viewFactory, IViewConfig config)
            : base(visualElement, viewFactory, config)
        {
            _text = VisualElement.Q<Label>("message-text");
            _footer = VisualElement.Q<Label>("message-footer");
        }

        private readonly Label _text;
        private readonly Label _footer;

        protected override void OnDataSet(Message data)
        {
            base.OnDataSet(data);

            _text.text = Data.Text;
            _footer.text = GetFooter();
        }

        private string GetFooter()
            => Data.User.Name + " " + Data.CreatedAt.Value.ToTimeAgoDescription();
    }
}