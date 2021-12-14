using Plugins.GetStreamIO.Unity.Scripts.Popups;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Factory for views
    /// </summary>
    public interface IViewFactory
    {
        MessageOptionsPopup CreateMessageOptionsPopup(MessageView messageView);
    }
}