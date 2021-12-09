using Plugins.GetStreamIO.Core;

namespace Plugins.GetStreamIO.Unity
{
    /// <summary>
    /// Context for view
    /// </summary>
    public interface IChatViewContext
    {
        IGetStreamChatClient Client { get; }
        IImageLoader ImageLoader { get; }
    }
}