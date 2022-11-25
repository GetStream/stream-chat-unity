using StreamChat.Core.LowLevelClient;
using StreamChat.SampleProject.Views;
using UnityEngine;

namespace StreamChat.SampleProject.Configs
{
    /// <summary>
    /// Asset to keep <see cref="IViewFactory"/> config
    /// </summary>
    [CreateAssetMenu(fileName = "AppConfig", menuName = StreamChatLowLevelClient.MenuPrefix + "View/Create app config asset", order = 1)]
    public class AppConfig : ScriptableObject, IAppConfig
    {
        public IEmojiConfig Emojis => _emojiConfig;

        public IViewFactoryConfig ViewFactoryConfig => _viewFactoryConfig;

        [SerializeField]
        private ViewFactoryConfig _viewFactoryConfig = new ViewFactoryConfig();

        [SerializeField]
        private EmojiConfigAsset _emojiConfig;
    }
}