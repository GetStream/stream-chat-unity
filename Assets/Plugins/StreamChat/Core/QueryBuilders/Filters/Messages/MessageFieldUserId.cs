using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by message author's user id (<see cref="IStreamMessage.User"/>.<see cref="IStreamUser.Id"/>).
    /// </summary>
    public sealed class MessageFieldUserId : BaseFieldToFilter
    {
        public override string FieldName => "user.id";

        public FieldFilterRule EqualsTo(string userId) => InternalEqualsTo(userId);

        public FieldFilterRule EqualsTo(IStreamUser user) => InternalEqualsTo(user.Id);

        public FieldFilterRule In(IEnumerable<string> userIds) => InternalIn(userIds);

        public FieldFilterRule In(params string[] userIds) => InternalIn(userIds);

        public FieldFilterRule In(IEnumerable<IStreamUser> users) => InternalIn(users.Select(_ => _.Id));

        public FieldFilterRule In(params IStreamUser[] users) => InternalIn(users.Select(_ => _.Id));
    }
}
