using System.Collections.Generic;
using System.Linq;
using StreamChat.SampleProject.Views;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StreamChat.SampleProject.Popups
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
            Destroy(gameObject);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (IsPopupUnderMousePointer())
            {
                return;
            }

            if (!HideOnPointerExit)
            {
                return;
            }

            Hide();
        }

        [SerializeField]
        private List<RaycastResult> _raycastBuffer = new List<RaycastResult>();

        protected TArgs SelfArgs { get; private set; }

        protected virtual void OnShow(TArgs args)
        {
        }

        protected virtual void OnHide(TArgs args)
        {
        }

        private bool IsPopupUnderMousePointer()
        {
            var pointerData = new PointerEventData (EventSystem.current)
            {
                pointerId = -1,
                position = InputSystem.MousePosition
            };

            EventSystem.current.RaycastAll(pointerData, _raycastBuffer);

            return _raycastBuffer.Any(_ => _.gameObject.transform.parent == transform);
        }
    }
}