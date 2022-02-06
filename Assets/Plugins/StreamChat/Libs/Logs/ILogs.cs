using System;

namespace StreamChat.Libs.Logs
{
    /// <summary>
    /// Handles logs
    /// </summary>
    public interface ILogs
    {
        void Info(string message);

        void Warning(string message);

        void Error(string message);

        void Exception(Exception exception);

        string Prefix { get; set; }
    }
}