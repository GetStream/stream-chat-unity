using System;
using System.Collections.Generic;
using System.Linq;
using StreamChat.Libs.Utils;

namespace StreamChat.Core.QueryBuilders.Filters
{
    public sealed class FieldFilterRule : IFieldFilterRule
    {
        public string Field { get; }
        public QueryOperatorType OperatorType { get; }
        public object Value { get; }

        public FieldFilterRule(string field, QueryOperatorType operatorType, bool value)
        {
            Field = field;
            OperatorType = operatorType;
            Value = value;
        }

        public FieldFilterRule(string field, QueryOperatorType operatorType, string value)
        {
            Field = field;
            OperatorType = operatorType;
            Value = value;
        }

        public FieldFilterRule(string field, QueryOperatorType operatorType, int value)
        {
            Field = field;
            OperatorType = operatorType;
            Value = value;
        }

        public FieldFilterRule(string field, QueryOperatorType operatorType, DateTime value)
        {
            Field = field;
            OperatorType = operatorType;
            // Store the raw DateTime so callers can pick the wire format at serialization time
            // (different Stream endpoints accept different RFC 3339 sub-forms - see StreamDateFormat).
            Value = value;
        }

        public FieldFilterRule(string field, QueryOperatorType operatorType, DateTimeOffset value)
        {
            Field = field;
            OperatorType = operatorType;
            // See note above about deferred date formatting.
            Value = value;
        }

        public FieldFilterRule(string field, QueryOperatorType operatorType, IEnumerable<string> value)
        {
            Field = field;
            OperatorType = operatorType;
            Value = value.ToArray();
        }

        public FieldFilterRule(string field, QueryOperatorType operatorType, IEnumerable<DateTime> value)
        {
            Field = field;
            OperatorType = operatorType;
            // See note above about deferred date formatting.
            Value = value.ToArray();
        }

        public FieldFilterRule(string field, QueryOperatorType operatorType, IEnumerable<DateTimeOffset> value)
        {
            Field = field;
            OperatorType = operatorType;
            // See note above about deferred date formatting.
            Value = value.ToArray();
        }

        /// <summary>
        /// Returns the filter entry using the default endpoint-portable date form
        /// (<see cref="StreamDateFormat.UtcOffset"/>). Callers targeting <c>POST /search</c>'s
        /// <c>message_filter_conditions</c> must use the format-aware overload
        /// (<see cref="GenerateFilterEntry(StreamDateFormat)"/>) with
        /// <see cref="StreamDateFormat.Utc"/>.
        /// </summary>
        //StreamTodo: research how to reduce allocation here
        public KeyValuePair<string, object> GenerateFilterEntry()
            => GenerateFilterEntry(StreamDateFormat.UtcOffset);

        /// <summary>
        /// Returns the filter entry, formatting any date values using <paramref name="dateFormat"/>.
        /// Non-date values are passed through untouched.
        /// </summary>
        internal KeyValuePair<string, object> GenerateFilterEntry(StreamDateFormat dateFormat)
            => new KeyValuePair<string, object>
            (
                Field, new Dictionary<string, object>
                {
                    {
                        OperatorType.ToOperatorKeyword(), FormatValueForWire(Value, dateFormat)
                    }
                }
            );

        private static object FormatValueForWire(object value, StreamDateFormat dateFormat)
        {
            if (value is DateTime dt)
            {
                return dt.ToStreamDateString(dateFormat);
            }

            if (value is DateTimeOffset dto)
            {
                return dto.ToStreamDateString(dateFormat);
            }

            if (value is DateTime[] dts)
            {
                return dts.Select(d => d.ToStreamDateString(dateFormat)).ToArray();
            }

            if (value is DateTimeOffset[] dtos)
            {
                return dtos.Select(d => d.ToStreamDateString(dateFormat)).ToArray();
            }

            return value;
        }
    }

    /// <summary>
    /// Internal helpers for serializing <see cref="IFieldFilterRule"/> instances to the wire
    /// dictionary with an explicit <see cref="StreamDateFormat"/>.
    ///
    /// <para>
    /// The public <see cref="IFieldFilterRule.GenerateFilterEntry"/> contract is intentionally
    /// parameterless to avoid breaking external implementations. SDK-internal call sites that
    /// need the <see cref="StreamDateFormat.Utc"/> (Z) form - currently only
    /// <c>POST /search</c>'s <c>message_filter_conditions</c> / <c>filter_conditions</c> - go
    /// through this helper. Anything implementing <see cref="IFieldFilterRule"/> that isn't the
    /// SDK's own <see cref="FieldFilterRule"/> transparently falls back to the parameterless
    /// path (i.e. <see cref="StreamDateFormat.UtcOffset"/>).
    /// </para>
    /// </summary>
    internal static class FieldFilterRuleExtensions
    {
        internal static KeyValuePair<string, object> GenerateFilterEntry(this IFieldFilterRule rule,
            StreamDateFormat dateFormat)
        {
            if (rule is FieldFilterRule concrete)
            {
                return concrete.GenerateFilterEntry(dateFormat);
            }

            return rule.GenerateFilterEntry();
        }
    }
}
