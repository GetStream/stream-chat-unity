using System;
using StreamChat.SampleProjects.UIToolkit.Config;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Views
{
    public abstract class BaseView : IDisposable
    {
        public VisualElement VisualElement { get; }

        public void Dispose() => OnDispose();

        protected IViewFactory Factory { get; }
        protected IViewConfig Config { get; }

        protected BaseView(VisualElement visualElement, IViewFactory viewFactory, IViewConfig config)
        {
            VisualElement = visualElement ?? throw new ArgumentNullException(nameof(visualElement));
            Factory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        protected virtual void OnDispose()
        {

        }
    }
}