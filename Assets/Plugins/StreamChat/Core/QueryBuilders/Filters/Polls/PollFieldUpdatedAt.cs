using System;

namespace StreamChat.Core.QueryBuilders.Filters.Polls
{
    /// <summary>
    /// Filter by Poll UpdatedAt timestamp
    /// </summary>
    public sealed class PollFieldUpdatedAt : BaseFieldToFilter
    {
        public override string FieldName => "updated_at";

        /// <summary>
        /// Return only polls updated AFTER the provided date
        /// </summary>
        public FieldFilterRule GreaterThan(DateTimeOffset date) => InternalGreaterThan(date);

        /// <summary>
        /// Return only polls updated AFTER OR ON the provided date
        /// </summary>
        public FieldFilterRule GreaterThanOrEquals(DateTimeOffset date) => InternalGreaterThanOrEquals(date);

        /// <summary>
        /// Return only polls updated BEFORE the provided date
        /// </summary>
        public FieldFilterRule LessThan(DateTimeOffset date) => InternalLessThan(date);

        /// <summary>
        /// Return only polls updated BEFORE OR ON the provided date
        /// </summary>
        public FieldFilterRule LessThanOrEquals(DateTimeOffset date) => InternalLessThanOrEquals(date);
    }
}

