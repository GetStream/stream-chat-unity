using System;
using StreamChat.SampleProjects.UIToolkit.Config;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Views
{
    public abstract class BaseDataView<TDataType> : BaseView
    {
        public TDataType Data { get; private set; }

        public void SetData(TDataType data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            OnDataSet(data);
        }

        protected BaseDataView(VisualElement visualElement, IViewFactory viewFactory, IViewConfig config)
            : base(visualElement, viewFactory, config)
        {
        }

        protected virtual void OnDataSet(TDataType data)
        {

        }
    }
}