using StreamChat.SampleProject.Views;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Popups
{
    /// <summary>
    /// Base class for fullscreen popups
    /// </summary>
    public class BaseFullscreenPopup : BaseView
    {
        protected override void OnInited()
        {
            base.OnInited();

            _closeButton.onClick.AddListener(OnCloseButtonClicked);

            OnShow();
        }

        protected override void OnDisposing()
        {
            _closeButton.onClick.RemoveListener(OnCloseButtonClicked);

            base.OnDisposing();
        }

        protected virtual void OnShow()
        {

        }

        protected void Hide()
            => State.HidePopup(this);

        [SerializeField]
        private Button _closeButton;

        private void OnCloseButtonClicked() => Hide();

    }
}