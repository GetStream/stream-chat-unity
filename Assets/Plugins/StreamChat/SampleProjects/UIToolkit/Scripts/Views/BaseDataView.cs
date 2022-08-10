using System;
using StreamChat.SampleProjects.UIToolkit.Config;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Views
{
    /// <summary>
    /// Base view for view representing single data model
    /// </summary>
    /// <typeparam name="TDataType">Type of data model</typeparam>
    /// <typeparam name="TVisualElement">Type of visual element representing this data model</typeparam>
    public abstract class BaseDataView<TDataType, TVisualElement> : BaseView<TVisualElement>
        where TVisualElement : VisualElement
    {
        public TDataType Data { get; private set; }

        public void SetData(TDataType data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            OnDataSet(data);
        }

        protected BaseDataView(TVisualElement visualElement, IViewFactory viewFactory, IViewConfig config)
            : base(visualElement, viewFactory, config)
        {
        }

        protected virtual void OnDataSet(TDataType data)
        {

        }
    }
}