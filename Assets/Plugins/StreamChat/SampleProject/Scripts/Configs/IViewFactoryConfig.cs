using StreamChat.SampleProject.Popups;
using StreamChat.SampleProject.Views;

namespace StreamChat.SampleProject.Configs
{
    /// <summary>
    /// Config for <see cref="IViewFactory"/>
    /// </summary>
    public interface IViewFactoryConfig
    {
        MessageOptionsPopup MessageOptionsPopupPrefab { get; }
        CreateNewChannelFormPopup CreateNewChannelFormPopupPrefab { get; }
        ErrorPopup ErrorPopupPrefab { get; }
    }
}