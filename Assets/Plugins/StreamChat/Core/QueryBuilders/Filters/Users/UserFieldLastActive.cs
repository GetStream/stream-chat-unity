using System;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Users
{
    /// <summary>
    /// Filter by <see cref="IStreamUser.LastActive"/>
    /// </summary>
    public sealed class UserFieldLastActive : BaseFieldToFilter
    {
        public override string FieldName => "last_active";

        /// <summary>
        /// Return only users where <see cref="IStreamUser.LastActive"/> is EQUAL to the provided one
        /// </summary>
        public FieldFilterRule EqualsTo(DateTime lastActive) => InternalEqualsTo(lastActive);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.LastActive"/> is EQUAL to the provided one
        /// </summary>
        public FieldFilterRule EqualsTo(DateTimeOffset updatedAt) => InternalEqualsTo(updatedAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.LastActive"/> is GREATER THAN the provided one
        /// </summary>
        public FieldFilterRule GreaterThan(DateTime updatedAt) => InternalGreaterThan(updatedAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.LastActive"/> is GREATER THAN the provided one
        /// </summary>
        public FieldFilterRule GreaterThan(DateTimeOffset updatedAt) => InternalGreaterThan(updatedAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.LastActive"/> is GREATER THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule GreaterThanOrEquals(DateTime updatedAt)
            => InternalGreaterThanOrEquals(updatedAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.LastActive"/> is GREATER THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule GreaterThanOrEquals(DateTimeOffset updatedAt)
            => InternalGreaterThanOrEquals(updatedAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.LastActive"/> is LESS THAN the provided one
        /// </summary>
        public FieldFilterRule LessThan(DateTime updatedAt) => InternalLessThan(updatedAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.LastActive"/> is LESS THAN the provided one
        /// </summary>
        public FieldFilterRule LessThan(DateTimeOffset updatedAt) => InternalLessThan(updatedAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.LastActive"/> is LESS THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule LessThanOrEquals(DateTime updatedAt) => InternalLessThanOrEquals(updatedAt);

        /// <summary>
        /// Return only users where <see cref="IStreamUser.LastActive"/> is LESS THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule LessThanOrEquals(DateTimeOffset updatedAt)
            => InternalLessThanOrEquals(updatedAt);
        
        /// <summary>
        /// Return only users where <see cref="IStreamUser.LastActive"/> is EQUAL to ANY of provided user Id
        /// </summary>
        public FieldFilterRule In(params DateTime[] updatedAt) => InternalIn(updatedAt);
        
        /// <summary>
        /// Return only users where <see cref="IStreamUser.LastActive"/> is EQUAL to ANY of provided user Id
        /// </summary>
        public FieldFilterRule In(params DateTimeOffset[] updatedAt) => InternalIn(updatedAt);
    }
}