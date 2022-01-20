using StreamChat.Unity.Scripts.Popups;

namespace StreamChat.Unity.Scripts
{
    /// <summary>
    /// Factory for views
    /// </summary>
    public interface IViewFactory
    {
        MessageOptionsPopup CreateMessageOptionsPopup(MessageView messageView);
    }
}