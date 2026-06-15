using System;
using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StreamChat.Core")]

namespace StreamChat.Libs.Utils
{
    /// <summary>
    /// Wire format choice for dates sent to the Stream API.
    ///
    /// The two forms are semantically identical (both encode UTC, RFC 3339), but individual
    /// Stream endpoints accept only one of them today:
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///     <see cref="UtcOffset"/> (<c>yyyy-MM-ddTHH:mm:ss.fff+00:00</c>) - required by
    ///     <c>POST /channels</c> <c>filter_conditions</c>. Sending the <c>Z</c> form causes the
    ///     server to return HTTP 500 with an empty error message.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     <see cref="Utc"/> (<c>yyyy-MM-ddTHH:mm:ss.fffZ</c>) - required by <c>POST /search</c>
    ///     <c>message_filter_conditions</c>. Sending the offset form is rejected with
    ///     <c>"field \"created_at\" expects type date"</c> (HTTP 400, code 4).
    ///     </description>
    ///   </item>
    /// </list>
    /// </summary>
    internal enum StreamDateFormat
    {
        /// <summary>
        /// Numeric-offset UTC form: <c>yyyy-MM-ddTHH:mm:ss.fff+00:00</c>.
        /// Used by <c>filter_conditions</c> on most endpoints (channels, users, threads, polls).
        /// </summary>
        UtcOffset,

        /// <summary>
        /// Canonical Zulu UTC form: <c>yyyy-MM-ddTHH:mm:ss.fffZ</c>.
        /// Required by <c>message_filter_conditions</c> on <c>POST /search</c>. Matches the
        /// format used by other Stream SDKs (e.g. <c>StreamDateFormatter</c> in stream-chat-android).
        /// </summary>
        Utc,
    }

    /// <summary>
    /// Formats <see cref="DateTime"/> / <see cref="DateTimeOffset"/> values for the Stream API.
    ///
    /// <para>
    /// Different Stream endpoints disagree on the acceptable RFC 3339 sub-form, so callers must
    /// pass an explicit <see cref="StreamDateFormat"/> when they know what the target endpoint
    /// expects. The parameterless overloads default to <see cref="StreamDateFormat.UtcOffset"/>,
    /// which is the form accepted by every endpoint except <c>POST /search</c>'s
    /// <c>message_filter_conditions</c>; that one path must opt into
    /// <see cref="StreamDateFormat.Utc"/>.
    /// </para>
    ///
    /// See <see cref="StreamDateFormat"/> for the endpoint-by-endpoint compatibility matrix.
    /// </summary>
    internal static class StreamDateFormatter
    {
        // "yyyy-MM-ddTHH:mm:ss.fff+00:00"
        private const string UtcOffsetFormat = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";

        // "yyyy-MM-ddTHH:mm:ss.fffZ" - equivalent to Java's "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'" used by the Android SDK.
        private const string UtcFormat = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";

        /// <summary>
        /// Formats <paramref name="dateTime"/> using the default endpoint-portable form
        /// (<see cref="StreamDateFormat.UtcOffset"/>). Use the overload taking an explicit
        /// <see cref="StreamDateFormat"/> when sending to <c>POST /search</c>'s
        /// <c>message_filter_conditions</c>, which only accepts <see cref="StreamDateFormat.Utc"/>.
        /// </summary>
        internal static string ToStreamDateString(this DateTime dateTime)
            => dateTime.ToStreamDateString(StreamDateFormat.UtcOffset);

        /// <summary>
        /// Formats <paramref name="dateTime"/> in the requested Stream API form.
        /// The value is normalised to UTC before formatting:
        /// <see cref="DateTimeKind.Utc"/> is used as-is, <see cref="DateTimeKind.Local"/> is
        /// converted via <see cref="DateTime.ToUniversalTime"/>, and
        /// <see cref="DateTimeKind.Unspecified"/> is assumed to already be UTC.
        /// </summary>
        internal static string ToStreamDateString(this DateTime dateTime, StreamDateFormat format)
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

            return utc.ToString(GetPattern(format), DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Formats <paramref name="dateTimeOffset"/> using the default endpoint-portable form
        /// (<see cref="StreamDateFormat.UtcOffset"/>). Use the overload taking an explicit
        /// <see cref="StreamDateFormat"/> when sending to <c>POST /search</c>'s
        /// <c>message_filter_conditions</c>, which only accepts <see cref="StreamDateFormat.Utc"/>.
        /// </summary>
        internal static string ToStreamDateString(this DateTimeOffset dateTimeOffset)
            => dateTimeOffset.ToStreamDateString(StreamDateFormat.UtcOffset);

        /// <summary>
        /// Formats <paramref name="dateTimeOffset"/> in the requested Stream API form.
        /// The value is converted to UTC before formatting; under
        /// <see cref="StreamDateFormat.UtcOffset"/> the wire output therefore always ends in
        /// <c>+00:00</c>, and under <see cref="StreamDateFormat.Utc"/> it ends in <c>Z</c>,
        /// regardless of the source offset.
        /// </summary>
        internal static string ToStreamDateString(this DateTimeOffset dateTimeOffset, StreamDateFormat format)
            => dateTimeOffset.UtcDateTime.ToString(GetPattern(format), DateTimeFormatInfo.InvariantInfo);

        private static string GetPattern(StreamDateFormat format)
        {
            switch (format)
            {
                case StreamDateFormat.UtcOffset:
                    return UtcOffsetFormat;
                case StreamDateFormat.Utc:
                    return UtcFormat;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}
