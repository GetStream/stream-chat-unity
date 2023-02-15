using System;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.CreatedAt"/>
    /// </summary>
    public sealed class ChannelFieldCreatedAt : BaseFieldToFilter
    {
        public override string FieldName => "created_at";

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedAt"/> is EQUAL to the provided one
        /// </summary>
        public FieldFilterRule EqualsTo(DateTime createdAt) => InternalEqualsTo(createdAt);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedAt"/> is EQUAL to the provided one
        /// </summary>
        public FieldFilterRule EqualsTo(DateTimeOffset createdAt) => InternalEqualsTo(createdAt);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedAt"/> is GREATER THAN the provided one
        /// </summary>
        public FieldFilterRule GreaterThan(DateTime createdAt) => InternalGreaterThan(createdAt);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedAt"/> is GREATER THAN the provided one
        /// </summary>
        public FieldFilterRule GreaterThan(DateTimeOffset createdAt) => InternalGreaterThan(createdAt);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedAt"/> is GREATER THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule GreaterThanOrEquals(DateTime createdAt)
            => InternalGreaterThanOrEquals(createdAt);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedAt"/> is GREATER THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule GreaterThanOrEquals(DateTimeOffset createdAt)
            => InternalGreaterThanOrEquals(createdAt);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedAt"/> is LESS THAN the provided one
        /// </summary>
        public FieldFilterRule LessThan(DateTime createdAt) => InternalLessThan(createdAt);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedAt"/> is LESS THAN the provided one
        /// </summary>
        public FieldFilterRule LessThan(DateTimeOffset createdAt) => InternalLessThan(createdAt);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedAt"/> is LESS THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule LessThanOrEquals(DateTime createdAt) => InternalLessThanOrEquals(createdAt);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedAt"/> is LESS THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule LessThanOrEquals(DateTimeOffset createdAt)
            => InternalLessThanOrEquals(createdAt);
    }
}