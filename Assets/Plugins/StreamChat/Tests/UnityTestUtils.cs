using System;
using System.Collections;
using System.Threading.Tasks;

namespace StreamChat.Tests
{
    /// <summary>
    /// Utils for testing purposes
    /// </summary>
    public static class UnityTestUtils
    {
        public static IEnumerator RunAsIEnumerator<TResponse>(this Task<TResponse> task, Action<TResponse> action)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                throw task.Exception;
            }

            action(task.Result);
        }
    }
}