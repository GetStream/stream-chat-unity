using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit
{
    public interface IViewConfig
    {
        VisualTreeAsset ChannelItemViewTemplate { get; }
    }
}