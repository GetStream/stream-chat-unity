using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when API request failed
    /// </summary>
    public class StreamApiException : Exception
    {
        public double? StatusCode { get; }
        public double? Code { get;  }
        public string Duration { get;  }
        public string ErrorMessage { get;  }
        public string MoreInfo { get;  }

        public IReadOnlyDictionary<string, string> ExceptionFields => _exceptionFields;

        internal StreamApiException(APIErrorInternalDTO apiError)
            : base($"{apiError.Message}, Error Code: {apiError.Code}, Http Status Code: {apiError.StatusCode}, More info: {apiError.MoreInfo}, Exception fields: {PrintExceptionFields(apiError)}")
        {
            StatusCode = apiError.StatusCode;
            Code = apiError.Code;
            Duration = apiError.Duration;
            ErrorMessage = apiError.Message;
            MoreInfo = apiError.MoreInfo;

            if (apiError.ExceptionFields != null && apiError.ExceptionFields.Count > 0)
            {
                _exceptionFields = new Dictionary<string, string>(apiError.ExceptionFields);
            }
        }

        private static readonly StringBuilder _sb = new StringBuilder();

        private readonly Dictionary<string, string> _exceptionFields;

        private static string PrintExceptionFields(APIErrorInternalDTO apiError)
        {
            if (apiError.ExceptionFields == null)
            {
                return "None";
            }

            _sb.Length = 0;

            var count = apiError.ExceptionFields.Count;
            var index = 0;
            foreach (var keyValuePair in apiError.ExceptionFields)
            {
                _sb.Append(keyValuePair.Key);
                _sb.Append(": ");
                _sb.Append(keyValuePair.Value);

                if (index < count - 1)
                {
                    _sb.Append(", ");
                }

                index++;
            }

            return _sb.ToString();
        }
    }

    /// <summary>
    /// Extensions for <see cref="StreamApiException"/>
    /// </summary>
    public static class StreamApiExceptionExt
    {
        public static void LogStreamExceptionIfFailed(this Task t, ILogs logger) => t.ContinueWith(_ =>
        {
            if (_.Exception.InnerException is StreamApiException streamApiException)
            {
                streamApiException.LogStreamApiExceptionDetails(logger);
            }

            logger.Exception(_.Exception);
        }, TaskContinuationOptions.OnlyOnFaulted);

        public static void LogStreamApiExceptionDetails(this StreamApiException exception, ILogs logger)
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

        private static void AppendLine(string name, string value)
        {
            _sb.Append(name);
            _sb.Append(": ");
            _sb.Append(value);
            _sb.Append(Environment.NewLine);
        }
    }
}