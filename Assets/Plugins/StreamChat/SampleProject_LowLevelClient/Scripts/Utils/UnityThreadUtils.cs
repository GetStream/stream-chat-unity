using System.Threading.Tasks;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Utils;

namespace StreamChat.SampleProject.Utils
{
    public static class UnityThreadUtils
    {
        public static void LogIfFailed(this Task t)
            => t.LogIfFailed(_logger);

        private static readonly ILogs _logger = new UnityLogs();
    }
}