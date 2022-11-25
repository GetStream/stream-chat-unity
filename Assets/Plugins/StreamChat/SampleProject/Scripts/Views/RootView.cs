namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Root view. This class finds and initializes all child <see cref="BaseView"/> instances
    /// </summary>
    /// <remarks>
    /// Does not handle dynamically spawned <see cref="BaseView"/> instances
    /// </remarks>
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