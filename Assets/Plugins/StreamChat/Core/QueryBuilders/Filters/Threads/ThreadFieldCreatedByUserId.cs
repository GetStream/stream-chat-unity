using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Threads
{
    /// <summary>
    /// Filter by Thread <c>created_by_user_id</c>
    /// </summary>
    public sealed class ThreadFieldCreatedByUserId : BaseFieldToFilter
    {
        public override string FieldName => "created_by_user_id";

        public FieldFilterRule EqualsTo(string userId) => InternalEqualsTo(userId);

        public FieldFilterRule EqualsTo(IStreamUser user) => InternalEqualsTo(user.Id);

        public FieldFilterRule In(IEnumerable<string> userIds) => InternalIn(userIds);

        public FieldFilterRule In(params string[] userIds) => InternalIn(userIds);

        public FieldFilterRule In(IEnumerable<IStreamUser> users) => InternalIn(users.Select(_ => _.Id));

        public FieldFilterRule In(params IStreamUser[] users) => InternalIn(users.Select(_ => _.Id));
    }
}
