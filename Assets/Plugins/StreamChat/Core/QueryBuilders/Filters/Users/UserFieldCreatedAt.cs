using System;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Users
{
    /// <summary>
    /// Filter by <see cref="IStreamUser.CreatedAt"/>
    /// </summary>
    public sealed class UserFieldCreatedAt : BaseFieldToFilter
    {
        public override string FieldName => "created_at";

        /// <summary>
        /// Return only users where <see cref="IStreamUser.CreatedAt"/> is EQUAL to the provided one
        /// </summary>
        public FieldFilterRule EqualsTo(DateTime createdAt) => InternalEqualsTo(createdAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.CreatedAt"/> is EQUAL to the provided one
        /// </summary>
        public FieldFilterRule EqualsTo(DateTimeOffset createdAt) => InternalEqualsTo(createdAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.CreatedAt"/> is GREATER THAN the provided one
        /// </summary>
        public FieldFilterRule GreaterThan(DateTime createdAt) => InternalGreaterThan(createdAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.CreatedAt"/> is GREATER THAN the provided one
        /// </summary>
        public FieldFilterRule GreaterThan(DateTimeOffset createdAt) => InternalGreaterThan(createdAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.CreatedAt"/> is GREATER THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule GreaterThanOrEquals(DateTime createdAt)
            => InternalGreaterThanOrEquals(createdAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.CreatedAt"/> is GREATER THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule GreaterThanOrEquals(DateTimeOffset createdAt)
            => InternalGreaterThanOrEquals(createdAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.CreatedAt"/> is LESS THAN the provided one
        /// </summary>
        public FieldFilterRule LessThan(DateTime createdAt) => InternalLessThan(createdAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.CreatedAt"/> is LESS THAN the provided one
        /// </summary>
        public FieldFilterRule LessThan(DateTimeOffset createdAt) => InternalLessThan(createdAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.CreatedAt"/> is LESS THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule LessThanOrEquals(DateTime createdAt) => InternalLessThanOrEquals(createdAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.CreatedAt"/> is LESS THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule LessThanOrEquals(DateTimeOffset createdAt)
            => InternalLessThanOrEquals(createdAt);
        
        /// <summary>
        /// Return only users where <see cref="IStreamUser.CreatedAt"/> is EQUAL to ANY of provided user Id
        /// </summary>
        public FieldFilterRule In(params DateTime[] createdAt) => InternalIn(createdAt);
        
        /// <summary>
        /// Return only users where <see cref="IStreamUser.CreatedAt"/> is EQUAL to ANY of provided user Id
        /// </summary>
        public FieldFilterRule In(params DateTimeOffset[] createdAt) => InternalIn(createdAt);
    }
}