using StreamChat.SampleProject.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Factory for views
    /// </summary>
    public interface IViewFactory
    {
        MessageOptionsPopup CreateMessageOptionsPopup(MessageView messageView);

        RectTransform PopupsContainer { get; }

        void CreateReactionEmoji(Image prefab, Transform container, string key);
    }
}