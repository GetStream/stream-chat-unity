using System;
using StreamChat.Core.State;
using StreamChat.Core;
using StreamChat.SampleProject_StateClient.Configs;
using StreamChat.SampleProject_StateClient.Inputs;
using StreamChat.SampleProject_StateClient.Utils;
using StreamChat.SampleProject_StateClient.Views;

namespace StreamChat.SampleProject_StateClient
{
    /// <inheritdoc />
    public class ChatViewContext : IChatViewContext
    {
        public IStreamChatClient Client { get; }
        public IImageLoader ImageLoader { get; }
        public IViewFactory Factory { get; }
        public IInputSystem InputSystem { get; }

        public IChatState State { get; }
        public IAppConfig AppConfig { get; }

        public ChatViewContext(IStreamChatClient client, IImageLoader imageLoader, ViewFactory viewFactory,
            IInputSystem inputSystem, IAppConfig appConfig)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            ImageLoader = imageLoader ?? throw new ArgumentNullException(nameof(imageLoader));
            Factory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            InputSystem = inputSystem ?? throw new ArgumentNullException(nameof(inputSystem));
            AppConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));

            State = new ChatState(client, Factory);
        }
    }
}