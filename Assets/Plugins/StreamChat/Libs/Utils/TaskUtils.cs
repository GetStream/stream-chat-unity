using System;
using System.Text;
using System.Threading.Tasks;
using StreamChat.Libs.Logs;
using UnityEngine;

namespace StreamChat.Libs.Utils
{
    // StreamTodo: make this internal
    public static class TaskUtils
    {
        public static void LogIfFailed(this Task t, ILogs logger)
            => t.ContinueWith(_ =>
                {
                    if (!_.IsFaulted)
                    {
                        return;
                    }

                    if (IsTransientNetworkException(_.Exception))
                    {
                        // A connectivity/transport failure (no network, a dropped/refused connection,
                        // a TLS or socket read failure, or a request timeout) on a fire-and-forget task
                        // is an expected, transient condition the reconnect flow recovers from. Log it as
                        // a warning instead of an exception so it does not flood crash/error reporting with
                        // handled, non-actionable noise. Genuine failures still surface as exceptions.
                        Debug.LogWarning(_.Exception.ToString());
                    }
                    else
                    {
                        Debug.LogException(_.Exception);
                    }
                },
                TaskScheduler.FromCurrentSynchronizationContext());

        /// <summary>
        /// Log exception thrown by this task with Debug.LogException
        /// </summary>
        /// <param name="t"></param>
        public static void LogIfFailed(this Task t)
            => t.ContinueWith(_ =>
                {
                    if (!_.IsFaulted)
                    {
                        return;
                    }

                    //Skip Debug.LogException because it doesn't print well nested exceptions, it just prints the most inner one
                    _sb.Length = 0;
                    Exception exception = _.Exception.Flatten();
                    while (exception != null)
                    {
                        if (exception is AggregateException)
                        {
                            exception = exception.InnerException;
                            continue;
                        }
                        _sb.AppendLine(exception.ToString());
                        _sb.AppendLine(exception.StackTrace);
                        _sb.AppendLine(Environment.NewLine);
                        _sb.AppendLine(Environment.NewLine);
                        
                        exception = exception.InnerException;
                    }

                    if (_sb.Length > 0)
                    {
                        // See LogIfFailed(Task, ILogs): connectivity/transport failures are expected,
                        // transient, and recovered by the reconnect flow, so log them as warnings rather
                        // than errors to avoid flooding crash/error reporting with non-actionable noise.
                        if (IsTransientNetworkException(_.Exception))
                        {
                            Debug.LogWarning(_sb.ToString());
                        }
                        else
                        {
                            Debug.LogError(_sb.ToString());
                        }
                        _sb.Length = 0;
                    }
                },
                TaskScheduler.FromCurrentSynchronizationContext());

        // True when a task faulted because of a connectivity / transport problem — no network, a
        // dropped/refused connection, a TLS or socket read failure, or a request timeout — rather than
        // a genuine application error. The SDK fire-and-forgets its connect / reconnect / restore
        // operations through LogIfFailed; when the device is offline these fail with such exceptions
        // and the reconnect flow recovers from them, so they are logged as warnings, not errors.
        private static bool IsTransientNetworkException(AggregateException aggregateException)
        {
            foreach (Exception inner in aggregateException.Flatten().InnerExceptions)
            {
                for (Exception e = inner; e != null; e = e.InnerException)
                {
                    if (e is System.Net.Http.HttpRequestException ||
                        e is System.Net.WebException ||
                        e is System.Net.Sockets.SocketException ||
                        e is System.IO.IOException ||
                        e is TimeoutException)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static  readonly StringBuilder _sb = new StringBuilder();
    }
}