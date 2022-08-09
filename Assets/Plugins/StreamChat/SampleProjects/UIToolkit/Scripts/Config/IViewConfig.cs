using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Config
{
    public interface IViewConfig
    {
        VisualTreeAsset ChannelItemViewTemplate { get; }
        VisualTreeAsset MessageItemViewTemplate { get; }
        VisualTreeAsset MessageInputFormViewTemplate { get; }
    }
}