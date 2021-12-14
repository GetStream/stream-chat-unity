namespace Plugins.GetStreamIO.Unity.Scripts.Popups
{
    /// <summary>
    /// Popup window base
    /// </summary>
    public abstract class BasePopup<TArgs> : BaseView
        where TArgs : struct, IPopupArgs
    {
        public bool HideOnMouseExit { get; private set; }

        public void Show(TArgs args)
        {
            _args = args;
            OnShow(args);
        }

        protected virtual void OnShow(TArgs args)
        {

        }

        protected virtual void OnHide(TArgs args)
        {

        }

        protected void OnMouseExit()
        {
            if (!HideOnMouseExit)
            {
                return;
            }

            OnHide(_args);
            gameObject.SetActive(false);
        }

        private TArgs _args;
    }
}