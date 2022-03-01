using StreamChat.SampleProject.Popups;
using UnityEngine;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Factory for views
    /// </summary>
    public interface IViewFactory
    {
        MessageOptionsPopup CreateMessageOptionsPopup(MessageView messageView);

        RectTransform PopupsContainer { get; }
    }
}