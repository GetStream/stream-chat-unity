using System;
using System.Collections.Generic;
using System.Text;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when an API request is rejected by the server. Inspect <see cref="StatusCode"/>
    /// (HTTP status) and <see cref="Code"/> (Stream's numeric error code) to react to a specific failure.
    /// The most common branches are exposed as <see cref="StreamApiExceptionExtensions"/> helpers
    /// (e.g. <see cref="StreamApiExceptionExtensions.IsRateLimitExceededError"/>).
    /// See <a href="https://getstream.io/chat/docs/unity/api-errors-response/">API Error Codes</a> for the full list.
    /// </summary>
    public class StreamApiException : Exception
    {
        // HTTP 400 — Bad Request
        public const int InputErrorHttpStatusCode = 400;
        public const int InputErrorStreamCode = 4;

        public const int MessageTooLongErrorHttpStatusCode = 400;
        public const int MessageTooLongErrorStreamCode = 20;

        public const int MessageModerationFailedErrorHttpStatusCode = 400;
        public const int MessageModerationFailedErrorStreamCode = 73;

        // HTTP 401 — Unauthorized
        public const int AuthenticationErrorHttpStatusCode = 401;
        public const int AuthenticationErrorStreamCode = 5;

        public const int TokenExpiredErrorHttpStatusCode = 401;
        public const int TokenExpiredErrorStreamCode = 40;

        public const int TokenNotValidYetErrorHttpStatusCode = 401;
        public const int TokenNotValidYetErrorStreamCode = 41;

        public const int TokenBeforeIssuedAtErrorHttpStatusCode = 401;
        public const int TokenBeforeIssuedAtErrorStreamCode = 42;

        public const int TokenSignatureInvalidErrorHttpStatusCode = 401;
        public const int TokenSignatureInvalidErrorStreamCode = 43;

        // HTTP 403 — Forbidden
        public const int PermissionDeniedErrorHttpStatusCode = 403;
        public const int PermissionDeniedErrorStreamCode = 17;

        public const int CooldownErrorHttpStatusCode = 403;
        public const int CooldownErrorStreamCode = 60;

        public const int NoAccessToChannelsErrorHttpStatusCode = 403;
        public const int NoAccessToChannelsErrorStreamCode = 70;

        public const int AppSuspendedErrorHttpStatusCode = 403;
        public const int AppSuspendedErrorStreamCode = 99;

        // HTTP 404 — Not Found
        public const int DoesNotExistErrorHttpStatusCode = 404;
        public const int DoesNotExistErrorStreamCode = 16;

        // HTTP 413 — Payload Too Large
        public const int PayloadTooBigErrorHttpStatusCode = 413;
        public const int PayloadTooBigErrorStreamCode = 22;

        // HTTP 429 — Too Many Requests
        public const int RateLimitErrorHttpStatusCode = 429;
        public const int RateLimitErrorStreamCode = 9;

        // HTTP 500 — Internal Server Error
        public const int InternalSystemErrorHttpStatusCode = 500;
        public const int InternalSystemErrorStreamCode = -1;

        //Stream
        public int? StatusCode { get; }
        public int? Code { get; }
        public string Duration { get; }
        public string ErrorMessage { get; }
        public string MoreInfo { get; }

        public IReadOnlyDictionary<string, string> ExceptionFields => _exceptionFields;

        internal StreamApiException(APIErrorInternalDTO apiError)
            : this(apiError.StatusCode, apiError.Code, apiError.Message, apiError.MoreInfo, apiError.Duration,
                apiError.ExceptionFields)
        {
        }

        /// <summary>
        /// Construct an exception representing a specific API error. Intended for integrators who need to
        /// exercise their own <see cref="StreamApiException"/> handling in unit tests (e.g. simulating a
        /// 403 / code 70 "no access to channels" response) without depending on the internal
        /// <see cref="APIErrorInternalDTO"/> type. Every argument maps directly to a public property of this type.
        /// </summary>
        public StreamApiException(
            int statusCode,
            int code,
            string errorMessage = null,
            string moreInfo = null,
            string duration = null,
            IReadOnlyDictionary<string, string> exceptionFields = null)
            : this((int?)statusCode, code, errorMessage, moreInfo, duration, exceptionFields)
        {
        }

        private StreamApiException(
            int? statusCode,
            int? code,
            string errorMessage,
            string moreInfo,
            string duration,
            IReadOnlyDictionary<string, string> exceptionFields)
            : base(BuildMessage(errorMessage, code, statusCode, moreInfo, exceptionFields))
        {
            StatusCode = statusCode;
            Code = code;
            Duration = duration;
            ErrorMessage = errorMessage;
            MoreInfo = moreInfo;

            if (exceptionFields != null && exceptionFields.Count > 0)
            {
                _exceptionFields = new Dictionary<string, string>(exceptionFields);
            }
        }

        private static readonly StringBuilder _sb = new StringBuilder();

        private readonly Dictionary<string, string> _exceptionFields;

        // Shared by both constructors so the message format stays identical however the exception is built.
        private static string BuildMessage(string message, int? code, int? statusCode, string moreInfo,
            IReadOnlyDictionary<string, string> exceptionFields)
            => $"{message}, Error Code: {code}, Http Status Code: {statusCode}, More info: {moreInfo}, Exception fields: {PrintExceptionFields(exceptionFields)}";

        private static string PrintExceptionFields(IReadOnlyDictionary<string, string> exceptionFields)
        {
            if (exceptionFields == null)
            {
                return "None";
            }

            _sb.Length = 0;

            var count = exceptionFields.Count;
            var index = 0;
            foreach (var keyValuePair in exceptionFields)
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
    /// Helpers for branching on the most common <see cref="StreamApiException"/> cases. Each
    /// helper matches both the HTTP status and the Stream numeric error code so it stays
    /// correct if the API ever reuses a status code for a different error.
    /// </summary>
    public static class StreamApiExceptionExtensions
    {
        /// <summary>
        /// HTTP 429 / Stream code 9. The caller exceeded the rate limit. Back off (typically with
        /// exponential delay) before retrying.
        /// </summary>
        public static bool IsRateLimitExceededError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.RateLimitErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.RateLimitErrorHttpStatusCode;

        /// <summary>
        /// HTTP 403 / Stream code 60. The user sent a message while the channel's slow-mode cooldown
        /// was active. Read <c>IStreamChannel.Cooldown</c> (in seconds) and gate the send UI for that
        /// many seconds before allowing another send.
        /// </summary>
        public static bool IsCooldownError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.CooldownErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.CooldownErrorHttpStatusCode;

        /// <summary>
        /// HTTP 403 / Stream code 17. The user lacks permission for the attempted action. Hide or
        /// disable the corresponding UI control; do not retry.
        /// </summary>
        public static bool IsPermissionDeniedError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.PermissionDeniedErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.PermissionDeniedErrorHttpStatusCode;

        /// <summary>
        /// HTTP 403 / Stream code 70. The user has no access to the requested channel(s). Hide the
        /// channel from the local list or show an "access denied" placeholder.
        /// </summary>
        public static bool IsNoAccessToChannelsError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.NoAccessToChannelsErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.NoAccessToChannelsErrorHttpStatusCode;

        /// <summary>
        /// HTTP 403 / Stream code 99. The Stream application has been suspended. Surface a
        /// service-unavailable UI; the integration is offline until the app is reactivated.
        /// </summary>
        public static bool IsAppSuspendedError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.AppSuspendedErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.AppSuspendedErrorHttpStatusCode;

        /// <summary>
        /// HTTP 401 / Stream code 5. Generic authentication failure (bad / missing token, wrong API
        /// key, etc.). If you connected with an <c>ITokenProvider</c> the SDK refreshes automatically;
        /// otherwise reconnect with a fresh token.
        /// </summary>
        public static bool IsAuthenticationError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.AuthenticationErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.AuthenticationErrorHttpStatusCode;

        /// <summary>
        /// HTTP 401 / Stream code 40. The user token has expired. With an <c>ITokenProvider</c>
        /// configured the SDK refreshes the token automatically; otherwise reconnect with a fresh
        /// token from your backend.
        /// </summary>
        public static bool IsTokenExpiredError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.TokenExpiredErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.TokenExpiredErrorHttpStatusCode;

        /// <summary>
        /// True for any of the token-related auth errors (expired / not-valid-yet / before-issued-at /
        /// signature-invalid). Use this when you do not need to distinguish between them.
        /// </summary>
        public static bool IsTokenError(this StreamApiException streamApiException)
            => streamApiException.StatusCode == 401 &&
               (streamApiException.Code == StreamApiException.TokenExpiredErrorStreamCode ||
                streamApiException.Code == StreamApiException.TokenNotValidYetErrorStreamCode ||
                streamApiException.Code == StreamApiException.TokenBeforeIssuedAtErrorStreamCode ||
                streamApiException.Code == StreamApiException.TokenSignatureInvalidErrorStreamCode);

        /// <summary>
        /// HTTP 404 / Stream code 16. The targeted resource (channel, message, user, …) does not
        /// exist or was deleted. Refresh the local view; do not retry.
        /// </summary>
        public static bool IsDoesNotExistError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.DoesNotExistErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.DoesNotExistErrorHttpStatusCode;

        /// <summary>
        /// HTTP 400 / Stream code 4. The request payload was invalid. Inspect
        /// <c>ExceptionFields</c> for per-field validation messages and surface them to the user.
        /// </summary>
        public static bool IsInputError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.InputErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.InputErrorHttpStatusCode;

        /// <summary>
        /// HTTP 400 / Stream code 20. The outgoing message exceeded the channel-type's
        /// <c>max_message_length</c>. Show a character-limit error to the sender.
        /// </summary>
        public static bool IsMessageTooLongError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.MessageTooLongErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.MessageTooLongErrorHttpStatusCode;

        /// <summary>
        /// HTTP 400 / Stream code 73. The message was rejected by moderation. Show a "your message
        /// was filtered" UI to the sender; do not retry the identical payload.
        /// </summary>
        public static bool IsMessageModerationFailedError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.MessageModerationFailedErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.MessageModerationFailedErrorHttpStatusCode;

        /// <summary>
        /// HTTP 413 / Stream code 22. The uploaded payload (e.g. file / image attachment) exceeded
        /// the maximum allowed size. Ask the user to choose a smaller file.
        /// </summary>
        public static bool IsPayloadTooBigError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.PayloadTooBigErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.PayloadTooBigErrorHttpStatusCode;

        /// <summary>
        /// HTTP 500 / Stream code -1. The server failed for an unspecified reason. Safe to retry
        /// after a short delay; surface a transient-failure UI if it persists.
        /// </summary>
        public static bool IsInternalSystemError(this StreamApiException streamApiException)
            => streamApiException.Code == StreamApiException.InternalSystemErrorStreamCode &&
               streamApiException.StatusCode == StreamApiException.InternalSystemErrorHttpStatusCode;
    }
}
