using System;
using StreamChat.SampleProjects.UIToolkit.Config;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Views
{
    /// <summary>
    /// Base view
    /// </summary>
    /// <typeparam name="TVisualElement">Type of Visual Element representing this view</typeparam>
    public abstract class BaseView<TVisualElement> : IDisposable
        where TVisualElement : VisualElement
    {
        public TVisualElement VisualElement { get; }

        public void Dispose() => OnDispose();

        protected IViewFactory Factory { get; }
        protected IViewConfig Config { get; }

        protected BaseView(TVisualElement visualElement, IViewFactory viewFactory, IViewConfig config)
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