using System;
using System.Collections.Generic;
using StreamChat.Core.State;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by an arbitrary custom message field (any top-level key the customer attached to the message).
    /// </summary>
    public sealed class MessageFieldCustom : BaseFieldToFilter
    {
        public override string FieldName { get; }

        public MessageFieldCustom(string customFieldName)
        {
            StreamAsserts.AssertNotNullOrEmpty(customFieldName, nameof(customFieldName));
            FieldName = customFieldName;
        }

        public FieldFilterRule EqualsTo(string value) => InternalEqualsTo(value);
        public FieldFilterRule EqualsTo(bool value) => InternalEqualsTo(value);
        public FieldFilterRule EqualsTo(int value) => InternalEqualsTo(value);
        public FieldFilterRule EqualsTo(DateTime value) => InternalEqualsTo(value);
        public FieldFilterRule EqualsTo(DateTimeOffset value) => InternalEqualsTo(value);

        public FieldFilterRule In(IEnumerable<string> values) => InternalIn(values);
        public FieldFilterRule In(params string[] values) => InternalIn(values);

        public FieldFilterRule GreaterThan(int value) => InternalGreaterThan(value);
        public FieldFilterRule GreaterThan(string value) => InternalGreaterThan(value);
        public FieldFilterRule GreaterThan(DateTime value) => InternalGreaterThan(value);
        public FieldFilterRule GreaterThan(DateTimeOffset value) => InternalGreaterThan(value);

        public FieldFilterRule GreaterThanOrEquals(int value) => InternalGreaterThanOrEquals(value);
        public FieldFilterRule GreaterThanOrEquals(string value) => InternalGreaterThanOrEquals(value);
        public FieldFilterRule GreaterThanOrEquals(DateTime value) => InternalGreaterThanOrEquals(value);
        public FieldFilterRule GreaterThanOrEquals(DateTimeOffset value) => InternalGreaterThanOrEquals(value);

        public FieldFilterRule LessThan(int value) => InternalLessThan(value);
        public FieldFilterRule LessThan(string value) => InternalLessThan(value);
        public FieldFilterRule LessThan(DateTime value) => InternalLessThan(value);
        public FieldFilterRule LessThan(DateTimeOffset value) => InternalLessThan(value);

        public FieldFilterRule LessThanOrEquals(int value) => InternalLessThanOrEquals(value);
        public FieldFilterRule LessThanOrEquals(string value) => InternalLessThanOrEquals(value);
        public FieldFilterRule LessThanOrEquals(DateTime value) => InternalLessThanOrEquals(value);
        public FieldFilterRule LessThanOrEquals(DateTimeOffset value) => InternalLessThanOrEquals(value);

        public FieldFilterRule Contains(string value) => InternalContains(value);

        public FieldFilterRule Exists(bool exists) => InternalExists(exists);
    }
}
