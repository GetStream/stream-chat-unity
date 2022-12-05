using System.Threading.Tasks;
using StreamChat.Libs.Logs;
using UnityEngine;

namespace StreamChat.Libs.Utils
{
    public static class TaskUtils
    {
        public static void LogIfFailed(this Task t, ILogs logger) 
            => t.ContinueWith(_ => logger.Exception(_.Exception.Flatten()),
            TaskContinuationOptions.OnlyOnFaulted);
        
        /// <summary>
        /// Log exception thrown by this task with Debug.LogException
        /// </summary>
        /// <param name="t"></param>
        public static void LogIfFailed(this Task t) 
            => t.ContinueWith(_ => Debug.LogException(_.Exception.Flatten()),
            TaskContinuationOptions.OnlyOnFaulted);
    }
}