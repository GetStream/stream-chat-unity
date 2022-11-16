using StreamChat.Core.State;
using StreamChat.Core;
using StreamChat.SampleProject_StateClient.Configs;
using StreamChat.SampleProject_StateClient.Inputs;
using StreamChat.SampleProject_StateClient.Utils;
using StreamChat.SampleProject_StateClient.Views;

namespace StreamChat.SampleProject_StateClient
{
    /// <summary>
    /// Context for view with state and common services
    /// </summary>
    public interface IChatViewContext
    {
        IStreamChatClient Client { get; }
        IImageLoader ImageLoader { get; }
        IViewFactory Factory { get; }
        IChatState State { get; }
        IInputSystem InputSystem { get; }
        IAppConfig AppConfig { get; }
    }
}