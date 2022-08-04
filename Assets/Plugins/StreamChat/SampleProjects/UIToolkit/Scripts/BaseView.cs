using System;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit
{
    public abstract class BaseView : IDisposable
    {
        public VisualElement VisualElement { get; }

        public BaseView(VisualElement visualElement, IViewFactory viewFactory, IViewConfig config)
        {
            VisualElement = visualElement ?? throw new ArgumentNullException(nameof(visualElement));
            Factory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void Dispose() => OnDispose();

        protected IViewFactory Factory { get; }
        protected IViewConfig Config { get; }

        protected virtual void OnDispose()
        {

        }
    }
}