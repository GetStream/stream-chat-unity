using StreamChat.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Config
{
    [CreateAssetMenu(fileName = "AppConfig", menuName = StreamChatClient.MenuPrefix + "Samples/UI Toolkit/Create view config asset", order = 1)]
    public class ViewConfig : ScriptableObject, IViewConfig
    {
        public VisualTreeAsset ChannelItemViewTemplate => _channelItemViewTemplate;
        public VisualTreeAsset MessageItemViewTemplate => _messageItemViewTemplate;
        public VisualTreeAsset MessageInputFormViewTemplate => _messageInputFormViewTemplate;

        [SerializeField]
        private VisualTreeAsset _channelItemViewTemplate;

        [SerializeField]
        private VisualTreeAsset _messageItemViewTemplate;

        [SerializeField]
        private VisualTreeAsset _messageInputFormViewTemplate;
    }
}