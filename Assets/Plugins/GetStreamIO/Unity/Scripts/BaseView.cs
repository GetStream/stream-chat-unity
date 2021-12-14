using System;
using Plugins.GetStreamIO.Core;
using UnityEngine;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Base class for view
    /// </summary>
    public abstract class BaseView : MonoBehaviour
    {
        public void Init(IChatViewContext viewContext)
        {
            ViewContext = viewContext ?? throw new ArgumentNullException(nameof(viewContext));

            OnInited();
        }

        protected IGetStreamChatClient Client => ViewContext.Client;
        protected IImageLoader ImageLoader => ViewContext.ImageLoader;
        protected IViewFactory Factory => ViewContext.Factory;
        protected IChatViewContext ViewContext { get; private set; }

        protected void OnDestroy() => OnDisposing();

        protected virtual void OnInited()
        {

        }

        protected virtual void OnDisposing()
        {

        }
    }
}