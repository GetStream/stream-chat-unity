using StreamChat.SampleProject.Popups;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Factory for views
    /// </summary>
    public interface IViewFactory
    {
        MessageOptionsPopup CreateMessageOptionsPopup(MessageView messageView);
    }
}