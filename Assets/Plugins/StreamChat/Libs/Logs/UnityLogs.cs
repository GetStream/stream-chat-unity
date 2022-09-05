using System;
using UnityEngine;

namespace StreamChat.Libs.Logs
{
    /// <summary>
    /// Unity <see cref="ILogs"/>
    /// </summary>
    public class UnityLogs : ILogs
    {
        public enum LogLevel
        {
            Disabled = 0,
            Info = 1 << 0,
            Warning = 1 << 1,
            Error = 1 << 2,
            Exception = 1 << 3,
            FailureOnly = Error | Exception,
            All = Info | Warning | Error | Exception
        }

        public UnityLogs(LogLevel logLevel = LogLevel.All)
        {
            _logLevel = logLevel;
        }

        public string Prefix { get; set; }

        public void Info(string message)
        {
            if ((_logLevel & LogLevel.Info) != 0)
            {
                Debug.Log(Prefix + message);
            }
        }

        public void Warning(string message)
        {
            if ((_logLevel & LogLevel.Warning) != 0)
            {
                Debug.LogWarning(Prefix + message);
            }
        }

        public void Error(string message)
        {
            if ((_logLevel & LogLevel.Error) != 0)
            {
                Debug.LogError(Prefix + message);
            }
        }

        public void Exception(Exception exception)
        {
            if ((_logLevel & LogLevel.Exception) != 0)
            {
                Debug.LogException(exception);
            }
        }

        private readonly LogLevel _logLevel;
    }
}