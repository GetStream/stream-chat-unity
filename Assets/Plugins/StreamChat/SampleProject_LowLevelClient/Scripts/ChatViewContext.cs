using System;
using StreamChat.Core;
using StreamChat.Core.LowLevelClient;
using StreamChat.SampleProject.Inputs;
using StreamChat.SampleProject.Configs;
using StreamChat.SampleProject.Utils;
using StreamChat.SampleProject.Views;
using UnityEngine;

namespace StreamChat.SampleProject
{
    /// <inheritdoc />
    public class ChatViewContext : IChatViewContext
    {
        public IStreamChatLowLevelClient LowLevelClient { get; }
        public IImageLoader ImageLoader { get; }
        public IViewFactory Factory { get; }
        public IInputSystem InputSystem { get; }

        public IChatState State { get; }
        public IAppConfig AppConfig { get; }

        public ChatViewContext(IStreamChatLowLevelClient lowLevelClient, IImageLoader imageLoader, ViewFactory viewFactory,
            IInputSystem inputSystem, IAppConfig appConfig)
        {
            LowLevelClient = lowLevelClient ?? throw new ArgumentNullException(nameof(lowLevelClient));
            ImageLoader = imageLoader ?? throw new ArgumentNullException(nameof(imageLoader));
            Factory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            InputSystem = inputSystem ?? throw new ArgumentNullException(nameof(inputSystem));
            AppConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));

            State = new ChatState(lowLevelClient, Factory);
        }
    }
}