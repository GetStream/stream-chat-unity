#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.LowLevelClient;
using UnityEditor;
using UnityEngine;

namespace StreamChat.Tests
{
    /// <summary>
    /// Utils for testing purposes
    /// </summary>
    internal static class UnityTestUtils
    {
        public static IEnumerator RunAsIEnumerator<TResponse>(this Task<TResponse> task,
            Action<TResponse> onSuccess = null, Action<Exception> onFaulted = null)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                if (onFaulted != null)
                {
                    onFaulted(task.Exception);
                    yield break;
                }
                else
                {
                    if (task.Exception is AggregateException aggregateException &&
                        aggregateException.InnerExceptions.Count == 1)
                    {
                        throw task.Exception.InnerException;
                    }

                    throw task.Exception;
                }
            }

            onSuccess?.Invoke(task.Result);
        }

        public static IEnumerator RunAsIEnumerator(this Task task,
            Action onSuccess = null, Action<Exception> onFaulted = null, IStreamChatClient statefulClient = null)
        {
            while (!task.IsCompleted)
            {
                statefulClient?.Update();
                yield return null;
            }

            if (task.IsFaulted)
            {
                if (onFaulted != null)
                {
                    onFaulted(task.Exception);
                    yield break;
                }
                
                if (task.Exception is AggregateException aggregateException &&
                    aggregateException.InnerExceptions.Count == 1)
                {
                    throw task.Exception.InnerException;
                }

                throw task.Exception;
            }

            onSuccess?.Invoke();
        }

        public static IEnumerator WaitForClientToConnect(this IStreamChatLowLevelClient lowLevelClient)
        {
            const float MaxTimeToConnect = 3;
            var timeStarted = EditorApplication.timeSinceStartup;

            while (true)
            {
                var elapsed = EditorApplication.timeSinceStartup - timeStarted;

                if (elapsed > MaxTimeToConnect)
                {
                    Debug.LogError("Waiting for connection exceeded max time. Terminating");
                    break;
                }

                lowLevelClient.Update(0.1f);

                if (lowLevelClient.ConnectionState == ConnectionState.Connecting)
                {
                    yield return null;
                }

                if (lowLevelClient.ConnectionState == ConnectionState.Connected)
                {
                    break;
                }

                if (lowLevelClient.ConnectionState == ConnectionState.Disconnected)
                {
                    Debug.LogError("Client disconnected when waiting for connection. Terminating");
                    break;
                }
            }
        }

        public static IEnumerator RunTaskAsEnumerator(this Task task, IStreamChatLowLevelClient client)
        {
            while (!task.IsCompleted)
            {
                client.Update(0.1f);
                yield return null;
            }
            
            if (!task.IsFaulted)
            {
                yield break;
            }
            
            if (task.Exception is AggregateException aggregateException &&
                aggregateException.InnerExceptions.Count == 1)
            {
                throw task.Exception.InnerException;
            }

            throw task.Exception;
        }

        public static IEnumerator WaitForSeconds(float seconds)
        {
            var timeStarted = Time.realtimeSinceStartup;

            while (Time.realtimeSinceStartup - timeStarted < seconds)
            {
                yield return null;
            }
        }
    }
}
#endif