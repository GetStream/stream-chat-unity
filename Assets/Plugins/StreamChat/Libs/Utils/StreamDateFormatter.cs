using System;
using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StreamChat.Core")]

namespace StreamChat.Libs.Utils
{
    /// <summary>
    /// Formats <see cref="DateTime"/> / <see cref="DateTimeOffset"/> values in the canonical
    /// Stream API format: <c>yyyy-MM-ddTHH:mm:ss.fffZ</c> (UTC, millisecond precision, literal "Z").
    ///
    /// This matches the format used by all other Stream SDKs (see
    /// <c>StreamDateFormatter</c> in stream-chat-android) and is the only form accepted by every
    /// Stream endpoint. In particular, the <c>/search</c> endpoint's <c>message_filter_conditions</c>
    /// rejects the numeric-offset form (<c>+00:00</c>) with
    /// <c>"field "created_at" expects type date"</c>, so any date sent to the API must go through
    /// this formatter to stay portable across endpoints.
    /// </summary>
    internal static class StreamDateFormatter
    {
        // Equivalent to Java's "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'" used by the Android SDK.
        private const string DateFormat = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";

        /// <summary>
        /// Formats <paramref name="dateTime"/> in the canonical Stream API format.
        /// The value is normalised to UTC before formatting:
        /// <see cref="DateTimeKind.Utc"/> is used as-is, <see cref="DateTimeKind.Local"/> is
        /// converted via <see cref="DateTime.ToUniversalTime"/>, and
        /// <see cref="DateTimeKind.Unspecified"/> is assumed to already be UTC.
        /// </summary>
        internal static string ToStreamDateString(this DateTime dateTime)
        {
            DateTime utc;
            switch (dateTime.Kind)
            {
                case DateTimeKind.Utc:
                    utc = dateTime;
                    break;
                case DateTimeKind.Local:
                    utc = dateTime.ToUniversalTime();
                    break;
                default:
                    utc = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                    break;
            }

            return utc.ToString(DateFormat, DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Formats <paramref name="dateTimeOffset"/> in the canonical Stream API format.
        /// The value is converted to UTC before formatting, so the wire output always ends in
        /// <c>Z</c> regardless of the source offset.
        /// </summary>
        internal static string ToStreamDateString(this DateTimeOffset dateTimeOffset)
            => dateTimeOffset.UtcDateTime.ToString(DateFormat, DateTimeFormatInfo.InvariantInfo);
    }
}
