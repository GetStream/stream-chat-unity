using System.Threading.Tasks;
using UnityEngine;

namespace StreamChat.SampleProjects.UIToolkit
{
    public static class TaskUtils
    {
        public static void LogOnFaulted(this Task task) => task.ContinueWith(_ => Debug.LogException(_.Exception),
            TaskContinuationOptions.OnlyOnFaulted);
    }
}