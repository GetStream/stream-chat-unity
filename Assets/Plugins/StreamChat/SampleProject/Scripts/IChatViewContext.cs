using StreamChat.Core;
using StreamChat.SampleProject.Inputs;
using StreamChat.SampleProject.Scripts.Utils;
using StreamChat.SampleProject.Views;

namespace StreamChat.SampleProject
{
    /// <summary>
    /// Context for view with state and common services
    /// </summary>
    public interface IChatViewContext
    {
        IStreamChatClient Client { get; }
        IImageLoader ImageLoader { get; }
        ViewFactory Factory { get; }
        IChatState State { get; }
        IInputSystem InputSystem { get; }
    }
}