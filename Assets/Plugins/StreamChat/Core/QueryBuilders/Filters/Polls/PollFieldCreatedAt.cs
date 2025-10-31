using System;

namespace StreamChat.Core.QueryBuilders.Filters.Polls
{
    /// <summary>
    /// Filter by Poll CreatedAt timestamp
    /// </summary>
    public sealed class PollFieldCreatedAt : BaseFieldToFilter
    {
        public override string FieldName => "created_at";

        /// <summary>
        /// Return only polls created AFTER the provided date
        /// </summary>
        public FieldFilterRule GreaterThan(DateTimeOffset date) => InternalGreaterThan(date);

        /// <summary>
        /// Return only polls created AFTER OR ON the provided date
        /// </summary>
        public FieldFilterRule GreaterThanOrEquals(DateTimeOffset date) => InternalGreaterThanOrEquals(date);

        /// <summary>
        /// Return only polls created BEFORE the provided date
        /// </summary>
        public FieldFilterRule LessThan(DateTimeOffset date) => InternalLessThan(date);

        /// <summary>
        /// Return only polls created BEFORE OR ON the provided date
        /// </summary>
        public FieldFilterRule LessThanOrEquals(DateTimeOffset date) => InternalLessThanOrEquals(date);
    }
}

