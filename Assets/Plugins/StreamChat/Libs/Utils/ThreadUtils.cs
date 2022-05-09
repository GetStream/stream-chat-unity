using System.Threading.Tasks;
using StreamChat.Libs.Logs;

namespace StreamChat.Libs.Utils
{
    public static class ThreadUtils
    {
        public static void LogIfFailed(this Task t, ILogs logger) => t.ContinueWith(_ => logger.Exception(_.Exception.Flatten()),
            TaskContinuationOptions.OnlyOnFaulted);
    }
}