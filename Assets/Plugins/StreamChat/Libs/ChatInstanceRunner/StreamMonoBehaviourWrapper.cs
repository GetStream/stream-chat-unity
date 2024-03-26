using System;
using System.Collections;
using UnityEngine;

namespace StreamChat.Libs.ChatInstanceRunner
{
    /// <summary>
    /// Wrapper to hide the <see cref="UnityStreamChatClientRunner"/> from Unity's inspector dropdowns and Unity search functions like Object.FindObjectsOfType<MonoBehaviour>(); 
    /// </summary>
    public sealed class StreamMonoBehaviourWrapper
    {
        /// <summary>
        /// This is a MonoBehaviour wrapper that will pass Unity Engine callbacks to the Stream Chat Client
        /// </summary>
        public sealed class UnityStreamChatClientRunner : MonoBehaviour, IStreamChatClientRunner
        {
            public void RunChatInstance(IStreamChatClientEventsListener streamChatInstance)
            {
                if (!Application.isPlaying)
                {
                    Debug.LogWarning($"Application is not playing. The MonoBehaviour {nameof(UnityStreamChatClientRunner)} wrapper will not execute." +
                              $" You need to call Stream Chat Client's {nameof(IStreamChatClientEventsListener.Update)} and {nameof(IStreamChatClientEventsListener.Destroy)} by yourself");
                    DestroyImmediate(gameObject);
                    return;
                }
                
                _streamChatInstance = streamChatInstance ?? throw new ArgumentNullException(nameof(streamChatInstance));
                _streamChatInstance.Disposed += OnStreamChatInstanceDisposed;
                StartCoroutine(UpdateCoroutine());
            }

            private IStreamChatClientEventsListener _streamChatInstance;
            
            // Called by Unity
            private void Awake()
            {
                DontDestroyOnLoad(gameObject);
            }

            // Called by Unity
            private void OnDestroy()
            {
                if (_streamChatInstance == null)
                {
                    return;
                }

                _streamChatInstance.Disposed -= OnStreamChatInstanceDisposed;
                StopCoroutine(UpdateCoroutine());
                _streamChatInstance.Destroy();
                _streamChatInstance = null;
            }

            private IEnumerator UpdateCoroutine()
            {
                while (_streamChatInstance != null)
                {
                    _streamChatInstance.Update();
                    yield return null;
                }
            }

            private void OnStreamChatInstanceDisposed()
            {
                if (_streamChatInstance == null)
                {
                    return;
                }

                _streamChatInstance.Disposed -= OnStreamChatInstanceDisposed;
                _streamChatInstance = null;
                StopCoroutine(UpdateCoroutine());

#if STREAM_DEBUG_ENABLED
                Debug.Log($"Stream Chat Client Disposed - destroy {nameof(UnityStreamChatClientRunner)} instance");
#endif
                Destroy(gameObject);
            }

        }
    }
}