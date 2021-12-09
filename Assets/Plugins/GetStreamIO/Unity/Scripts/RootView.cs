namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Root view
    /// </summary>
    public class RootView : BaseView
    {
        protected override void OnInited()
        {
            base.OnInited();

            foreach (var childView in GetComponentsInChildren<BaseView>())
            {
                if (childView == this)
                {
                    continue;
                }

                childView.Init(ViewContext);
            }
        }
    }
}