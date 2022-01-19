using Plugins.GetStreamIO.Core;
using Plugins.GetStreamIO.Core.Utils;
using Plugins.GetStreamIO.Unity.Scripts;

namespace Plugins.GetStreamIO.Unity
{
    /// <summary>
    /// Context for view with state and common services
    /// </summary>
    public interface IChatViewContext
    {
        IGetStreamChatClient Client { get; }
        IImageLoader ImageLoader { get; }
        ViewFactory Factory { get; }
        IChatState State { get; }
    }
}