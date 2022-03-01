using StreamChat.SampleProject.Plugins.StreamChat.SampleProject.Scripts.Configs;
using StreamChat.SampleProject.Popups;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Config for <see cref="IViewFactory"/>
    /// </summary>
    public interface IViewFactoryConfig
    {
        MessageOptionsPopup MessageOptionsPopupPrefab { get; }
        IEmojiConfig EmojiConfig { get; }
    }
}