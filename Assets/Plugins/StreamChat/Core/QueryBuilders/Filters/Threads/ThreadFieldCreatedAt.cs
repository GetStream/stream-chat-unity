using System;

namespace StreamChat.Core.QueryBuilders.Filters.Threads
{
    /// <summary>
    /// Filter by Thread <c>created_at</c> timestamp
    /// </summary>
    public sealed class ThreadFieldCreatedAt : BaseFieldToFilter
    {
        public override string FieldName => "created_at";

        public FieldFilterRule GreaterThan(DateTimeOffset date) => InternalGreaterThan(date);
        public FieldFilterRule GreaterThanOrEquals(DateTimeOffset date) => InternalGreaterThanOrEquals(date);
        public FieldFilterRule LessThan(DateTimeOffset date) => InternalLessThan(date);
        public FieldFilterRule LessThanOrEquals(DateTimeOffset date) => InternalLessThanOrEquals(date);
        public FieldFilterRule EqualsTo(DateTimeOffset date) => InternalEqualsTo(date);
    }
}
