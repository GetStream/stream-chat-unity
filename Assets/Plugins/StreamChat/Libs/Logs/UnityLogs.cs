using System;
using UnityEngine;

namespace StreamChat.Libs.Logs
{
    /// <summary>
    /// Unity <see cref="ILogs"/>
    /// </summary>
    public class UnityLogs : ILogs
    {
        public string Prefix { get; set; }

        public void Info(string message) => Debug.Log(Prefix + message);

        public void Warning(string message) => Debug.LogWarning(Prefix + message);

        public void Error(string message) => Debug.LogError(Prefix + message);

        public void Exception(Exception exception) => Debug.LogException(exception);
    }
}