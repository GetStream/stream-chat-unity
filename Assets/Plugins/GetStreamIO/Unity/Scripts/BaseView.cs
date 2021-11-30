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
        public void Init(IGetStreamChatClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));

            OnInited();
        }

        protected IGetStreamChatClient Client { get; private set; }

        protected void OnDestroy() => OnDisposing();

        protected virtual void OnInited()
        {

        }

        protected virtual void OnDisposing()
        {

        }
    }
}