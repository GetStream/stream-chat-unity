using StreamChat.Unity.Scripts.Popups;

namespace StreamChat.Unity.Scripts
{
    /// <summary>
    /// Config for <see cref="IViewFactory"/>
    /// </summary>
    public interface IViewFactoryConfig
    {
        MessageOptionsPopup MessageOptionsPopupPrefab { get; }
    }
}