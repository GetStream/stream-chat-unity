using System.Threading.Tasks;
using StreamChat.Core.Exceptions;
using StreamChat.Libs.Logs;

namespace StreamChat.SampleProject.Utils
{
    /// <summary>
    /// Extensions for <see cref="StreamApiException"/>
    /// </summary>
    public static class StreamApiUnityExceptionExt
    {
        public static void LogStreamExceptionIfFailed(this Task t)
            => t.LogStreamExceptionIfFailed(_logger);

        private static readonly ILogs _logger = new UnityLogs();
    }
}

