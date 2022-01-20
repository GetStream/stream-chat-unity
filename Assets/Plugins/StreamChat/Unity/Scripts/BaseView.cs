using System;
using StreamChat.Core;
using StreamChat.Core.Utils;
using UnityEngine;

namespace StreamChat.Unity.Scripts
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

        protected IStreamChatClient Client => ViewContext.Client;
        protected IImageLoader ImageLoader => ViewContext.ImageLoader;
        protected IViewFactory Factory => ViewContext.Factory;
        protected IChatViewContext ViewContext { get; private set; }
        protected IChatState State => ViewContext.State;

        protected void OnDestroy() => OnDisposing();

        protected virtual void OnInited()
        {

        }

        protected virtual void OnDisposing()
        {

        }
    }
}