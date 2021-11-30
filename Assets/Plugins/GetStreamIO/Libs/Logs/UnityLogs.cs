using System;
using UnityEngine;

namespace Plugins.GetStreamIO.Libs.Logs
{
    /// <summary>
    /// Unity <see cref="ILogs"/>
    /// </summary>
    public class UnityLogs : ILogs
    {
        public void Info(string message) => Debug.Log(message);

        public void Warning(string message) => Debug.LogWarning(message);

        public void Error(string message) => Debug.LogError(message);

        public void Exception(Exception exception) => Debug.LogException(exception);
    }
}