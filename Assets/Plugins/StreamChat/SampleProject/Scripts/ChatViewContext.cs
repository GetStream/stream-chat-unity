using System;
using StreamChat.Core;
using StreamChat.SampleProject.Configs;
using StreamChat.SampleProject.Inputs;
using StreamChat.SampleProject.Utils;
using StreamChat.SampleProject.Views;

namespace StreamChat.SampleProject
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