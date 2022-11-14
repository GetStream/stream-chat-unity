﻿using System;
using StreamChat.Core;
using StreamChat.Core.State;
using StreamChat.SampleProject_StateClient.Inputs;
using StreamChat.SampleProject_StateClient.Utils;
using UnityEngine;

namespace StreamChat.SampleProject_StateClient.Views
{
    /// <summary>
    /// Base class for view
    /// </summary>
    public abstract class BaseView : MonoBehaviour
    {
        public void Init(IChatViewContext viewContext)
        {
            ViewContext = viewContext ?? throw new ArgumentNullException(nameof(viewContext));

            _isInited = true;
            OnInited();
        }

        protected IStreamChatStateClient Client => ViewContext.Client;
        protected IImageLoader ImageLoader => ViewContext.ImageLoader;
        protected IViewFactory Factory => ViewContext.Factory;
        protected IChatViewContext ViewContext { get; private set; }
        protected IChatState State => ViewContext.State;
        protected IInputSystem InputSystem => ViewContext.InputSystem;

        protected readonly string[] AllowedVideoFormats = new[] { "mp4", "mov", "mpg", "mpeg", "avi" };

        protected void OnDestroy()
        {
            if (_isInited)
            {
                OnDisposing();
            }
        }

        protected void Update()
        {
            if (!_isInited)
            {
                return;
            }

            OnUpdate();
        }

        protected virtual void OnInited()
        {

        }

        protected virtual void OnUpdate()
        {

        }

        protected virtual void OnDisposing()
        {

        }

        private bool _isInited;
    }
}