using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Polls
{
    /// <summary>
    /// Filter by Poll Creator User Id
    /// </summary>
    public sealed class PollFieldCreatedById : BaseFieldToFilter
    {
        public override string FieldName => "created_by_id";

        /// <summary>
        /// Return only polls where CreatedById is EQUAL to provided user Id
        /// </summary>
        public FieldFilterRule EqualsTo(string userId) => InternalEqualsTo(userId);

        /// <summary>
        /// Return only polls where CreatedById is EQUAL to ANY of provided user Ids
        /// </summary>
        public FieldFilterRule In(IEnumerable<string> userIds) => InternalIn(userIds);

        /// <summary>
        /// Return only polls where CreatedById is EQUAL to ANY of provided user Ids
        /// </summary>
        public FieldFilterRule In(params string[] userIds) => InternalIn(userIds);

        /// <summary>
        /// Return only polls where CreatedById is EQUAL to ANY of the provided users Id
        /// </summary>
        public FieldFilterRule In(IEnumerable<IStreamUser> users)
            => InternalIn(users.Select(_ => _.Id));

        /// <summary>
        /// Return only polls where CreatedById is EQUAL to ANY of the provided users Id
        /// </summary>
        public FieldFilterRule In(params IStreamUser[] users)
            => InternalIn(users.Select(_ => _.Id));
    }
}

