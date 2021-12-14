using UnityEngine.EventSystems;

namespace Plugins.GetStreamIO.Unity.Scripts.Popups
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
            _args = args;

            HideOnPointerExit = args.HideOnPointerExit;

            OnShow(args);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!HideOnPointerExit)
            {
                return;
            }

            OnHide(_args);
            gameObject.SetActive(false);
        }

        protected virtual void OnShow(TArgs args)
        {

        }

        protected virtual void OnHide(TArgs args)
        {

        }

        private TArgs _args;
    }
}