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

            // Called by Unity. Also fired with false when the player starts.
            private void OnApplicationPause(bool pauseStatus)
            {
                if (_streamChatInstance == null)
                {
                    return;
                }

#if UNITY_EDITOR
                // Play-mode pause / unfocus must not drop the socket, even if
                // DisconnectOnApplicationPause is true (including the player default).
                if (pauseStatus && !_loggedEditorPauseIgnored)
                {
                    _loggedEditorPauseIgnored = true;
                    Debug.LogWarning(
                        "DisconnectOnApplicationPause is ignored in the Unity Editor so play-mode pause / unfocus " +
                        "does not drop the socket. Call PauseConnectionAsync / ResumeConnectionAsync to test that path.");
                }

                return;
#else
                _streamChatInstance.OnApplicationPause(pauseStatus);
#endif
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

            private IStreamChatClientEventsListener _streamChatInstance;
            private bool _loggedEditorPauseIgnored;
        }
    }
}
