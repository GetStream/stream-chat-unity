using System;
using Plugins.GetStreamIO.Core;

namespace Plugins.GetStreamIO.Unity
{
    /// <summary>
    /// Default implementation of <see cref="IChatViewContext"/>
    /// </summary>
    public class ChatViewContext : IChatViewContext
    {
        public IGetStreamChatClient Client { get; }
        public IImageLoader ImageLoader { get; }

        public ChatViewContext(IGetStreamChatClient client, IImageLoader imageLoader)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            ImageLoader = imageLoader ?? throw new ArgumentNullException(nameof(imageLoader));
        }
    }
}