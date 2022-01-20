using System;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.Core.Utils;
using Plugins.GetStreamIO.Unity.Scripts;

namespace Plugins.GetStreamIO.Unity
{
    /// <summary>
    /// Default implementation of <see cref="IChatViewContext"/>
    /// </summary>
    public class ChatViewContext : IChatViewContext
    {
        public IStreamChatClient Client { get; }
        public IImageLoader ImageLoader { get; }
        public ViewFactory Factory { get; }

        public IChatState State { get; }

        public ChatViewContext(IStreamChatClient client, IImageLoader imageLoader, ViewFactory viewFactory)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            ImageLoader = imageLoader ?? throw new ArgumentNullException(nameof(imageLoader));
            Factory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));

            State = new ChatState(client);
        }
    }
}