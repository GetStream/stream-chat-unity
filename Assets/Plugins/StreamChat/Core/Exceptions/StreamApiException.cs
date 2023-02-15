using System;
using System.Collections.Generic;
using System.Text;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when API request failed
    /// </summary>
    public class StreamApiException : Exception
    {
        public const int RateLimitErrorHttpStatusCode = 429;
        public const int RateLimitErrorStreamCode = 9;

        //Stream
        public int? StatusCode { get; }
        public int? Code { get; }
        public string Duration { get; }
        public string ErrorMessage { get; }
        public string MoreInfo { get; }

        public IReadOnlyDictionary<string, string> ExceptionFields => _exceptionFields;

        internal StreamApiException(APIErrorInternalDTO apiError)
            : base(
                $"{apiError.Message}, Error Code: {apiError.Code}, Http Status Code: {apiError.StatusCode}, More info: {apiError.MoreInfo}, Exception fields: {PrintExceptionFields(apiError)}")
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
    public static class StreamApiExceptionExtensions
    {
        public static bool IsRateLimitExceededError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.RateLimitErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.RateLimitErrorHttpStatusCode;
    }
}