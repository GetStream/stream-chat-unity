using System;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by message <see cref="IStreamMessage.CreatedAt"/> timestamp.
    /// </summary>
    public sealed class MessageFieldCreatedAt : BaseFieldToFilter
    {
        public override string FieldName => "created_at";

        public FieldFilterRule EqualsTo(DateTime date) => InternalEqualsTo(date);
        public FieldFilterRule EqualsTo(DateTimeOffset date) => InternalEqualsTo(date);

        public FieldFilterRule GreaterThan(DateTime date) => InternalGreaterThan(date);
        public FieldFilterRule GreaterThan(DateTimeOffset date) => InternalGreaterThan(date);

        public FieldFilterRule GreaterThanOrEquals(DateTime date) => InternalGreaterThanOrEquals(date);
        public FieldFilterRule GreaterThanOrEquals(DateTimeOffset date) => InternalGreaterThanOrEquals(date);

        public FieldFilterRule LessThan(DateTime date) => InternalLessThan(date);
        public FieldFilterRule LessThan(DateTimeOffset date) => InternalLessThan(date);

        public FieldFilterRule LessThanOrEquals(DateTime date) => InternalLessThanOrEquals(date);
        public FieldFilterRule LessThanOrEquals(DateTimeOffset date) => InternalLessThanOrEquals(date);
    }
}
