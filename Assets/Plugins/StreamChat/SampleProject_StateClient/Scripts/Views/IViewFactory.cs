using StreamChat.SampleProject_StateClient.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject_StateClient.Views
{
    /// <summary>
    /// Factory for views
    /// </summary>
    public interface IViewFactory
    {
        RectTransform PopupsContainer { get; }

        MessageOptionsPopup CreateMessageOptionsPopup(MessageView messageView, IChatState state);

        void CreateEmoji(Image prefab, Transform container, string key);

        TPopup CreateFullscreenPopup<TPopup>()
            where TPopup : BaseFullscreenPopup;
    }
}