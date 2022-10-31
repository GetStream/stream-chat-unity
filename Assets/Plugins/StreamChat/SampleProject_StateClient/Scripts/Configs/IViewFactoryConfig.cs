using StreamChat.SampleProject_StateClient.Popups;
using StreamChat.SampleProject_StateClient.Views;

namespace StreamChat.SampleProject_StateClient.Configs
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