using System;
using System.Text;
using System.Threading.Tasks;
using StreamChat.Core.Exceptions;
using StreamChat.Libs;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// Stream Utility helper methods
    /// </summary>
    public static class StreamUtils
    {
        /// <summary>
        /// Run task as a callback
        /// </summary>
        /// <param name="onSuccess">Called when task succeeds. Contains response object</param>
        /// <param name="onFailure">Called when task fails. Contains thrown exception.</param>
        /// <typeparam name="TResponse">Type of response when task succeeds.</typeparam>
        public static void AsCallback<TResponse>(this Task<TResponse> task, Action<TResponse> onSuccess = null,
            Action<Exception> onFailure = null)
        {
            task.ContinueWith(_ =>
            {
                if (_.IsFaulted)
                {
                    onFailure?.Invoke(_.Exception);
                    return;
                }

                onSuccess?.Invoke(_.Result);
            });
        }

        /// <summary>
        /// Format and log exceptions thrown if this task failed
        /// </summary>
        public static void LogExceptionsOnFailed(this Task t)
            => LogExceptionsOnFailed(t, Logger);

        /// <summary>
        /// Format and log exception details
        /// </summary>
        public static void LogStreamExceptionDetails(this StreamApiException exception)
            => LogStreamExceptionDetails(exception, Logger);

        public static void LogStreamExceptionDetails(this StreamApiException exception, ILogs logger)
        {
            _sb.Append(nameof(StreamApiException));
            _sb.Append(":");
            _sb.Append(Environment.NewLine);

            if (exception.StatusCode.HasValue)
            {
                AppendLine(nameof(exception.StatusCode), exception.StatusCode.Value.ToString());
            }

            if (exception.Code.HasValue)
            {
                AppendLine(nameof(exception.Code), exception.Code.Value.ToString());
            }

            AppendLine(nameof(exception.Duration), exception.Duration);
            AppendLine(nameof(exception.ErrorMessage), exception.ErrorMessage);
            AppendLine(nameof(exception.MoreInfo), exception.MoreInfo);

            if (exception.ExceptionFields != null)
            {
                _sb.Append(nameof(exception.ExceptionFields));
                _sb.Append(":");
                _sb.Append(Environment.NewLine);

                foreach (var item in exception.ExceptionFields)
                {
                    AppendLine(item.Key, item.Value);
                }
            }

            logger.Exception(new Exception(_sb.ToString(), exception));
            _sb.Length = 0;
        }

        private static readonly StringBuilder _sb = new StringBuilder();
        private static readonly ILogs Logger = StreamDependenciesFactory.CreateLogger();

        private static void LogExceptionsOnFailed(this Task t, ILogs logger) => t.ContinueWith(_ =>
        {
            if (_.Exception.InnerException is StreamApiException streamApiException)
            {
                streamApiException.LogStreamExceptionDetails(logger);
            }

            logger.Exception(_.Exception);
        }, TaskContinuationOptions.OnlyOnFaulted);

        private static void AppendLine(string name, string value)
        {
            _sb.Append(name);
            _sb.Append(": ");
            _sb.Append(value);
            _sb.Append(Environment.NewLine);
        }
    }
}