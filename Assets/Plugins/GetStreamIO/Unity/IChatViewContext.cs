using Plugins.GetStreamIO.Core;
using Plugins.GetStreamIO.Unity.Scripts;

namespace Plugins.GetStreamIO.Unity
{
    /// <summary>
    /// Context for view
    /// </summary>
    public interface IChatViewContext
    {
        IGetStreamChatClient Client { get; }
        IImageLoader ImageLoader { get; }
        ViewFactory Factory { get; }
    }
}