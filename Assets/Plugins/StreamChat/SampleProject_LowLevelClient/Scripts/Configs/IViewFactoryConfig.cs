using StreamChat.SampleProject.Popups;

namespace StreamChat.SampleProject.Views
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