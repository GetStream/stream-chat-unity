using UnityEngine.EventSystems;

namespace StreamChat.Unity.Scripts.Popups
{
    /// <summary>
    /// Popup window base
    /// </summary>
    public abstract class BasePopup<TArgs> : BaseView, IPointerExitHandler
        where TArgs : struct, IPopupArgs
    {
        public bool HideOnPointerExit { get; private set; }

        public void Show(TArgs args)
        {
            SelfArgs = args;

            HideOnPointerExit = args.HideOnPointerExit;

            OnShow(args);
        }

        public void Hide()
        {
            OnHide(SelfArgs);
            gameObject.SetActive(false);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (!HideOnPointerExit)
            {
                return;
            }

            Hide();
        }

        protected TArgs SelfArgs { get; private set; }

        protected virtual void OnShow(TArgs args)
        {
        }

        protected virtual void OnHide(TArgs args)
        {
        }
    }
}