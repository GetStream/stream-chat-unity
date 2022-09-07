using System;
using System.Threading.Tasks;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// Extensions for Task
    /// </summary>
    public static class TaskExt
    {
        /// <summary>
        /// Run task as callback
        /// </summary>
        /// <param name="onSuccess">Called when task succeeds. Contains response object</param>
        /// <param name="onFailure">Called when task fails. Contains thrown exception.</param>
        /// <typeparam name="TResponse">Type of response when task succeeds.</typeparam>
        public static void AsCallback<TResponse>(this Task<TResponse> task, Action<TResponse> onSuccess = null, Action<Exception> onFailure = null)
        {
            task.ContinueWith(_ =>
            {
                if (_.IsFaulted)
                {
                    onFailure?.Invoke(_.Exception);
                    return;
                }

                onSuccess?.Invoke(_.Result);
            });
        }
    }
}