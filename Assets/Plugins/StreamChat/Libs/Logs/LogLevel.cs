namespace StreamChat.Libs.Logs
{
    /// <summary>
    /// Determines what type of logs are being logged by <see cref="ILogs"/>
    /// </summary>
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
}