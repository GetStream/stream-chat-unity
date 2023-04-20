using System;
using StreamChat.Libs.Logs;

namespace StreamChat.Core
{
    /// <summary>
    /// Log level of StreamChatClient.
    /// </summary>
    public enum StreamLogLevel
    {
        /// <summary>
        /// Logging is entirely disabled. This option is not recommended.
        /// </summary>
        Disabled,

        /// <summary>
        /// Only Errors and Exception. Recommended for production mode.
        /// </summary>
        FailureOnly,

        /// <summary>
        /// All logs will be emitted. Recommended for development or production mode.
        /// </summary>
        All,

        /// <summary>
        /// Additional logs will be emitted. Useful when debugging the StreamChatClient or internal WebSocket connection.
        /// </summary>
        Debug
    }

    /// <summary>
    /// Extensions for <see cref="StreamLogLevel"/>
    /// </summary>
    internal static class StreamLogsLevelExt
    {
        public static LogLevel ToLogLevel(this StreamLogLevel streamLogLevel)
        {
            switch (streamLogLevel)
            {
                case StreamLogLevel.Disabled: return LogLevel.Disabled;
                case StreamLogLevel.FailureOnly: return LogLevel.FailureOnly;
                case StreamLogLevel.All:
                case StreamLogLevel.Debug: return LogLevel.All;
                default:
                    throw new ArgumentOutOfRangeException(nameof(streamLogLevel), streamLogLevel, null);
            }
        }

        public static bool IsDebugEnabled(this StreamLogLevel streamLogLevel) => streamLogLevel == StreamLogLevel.Debug;
    }
}