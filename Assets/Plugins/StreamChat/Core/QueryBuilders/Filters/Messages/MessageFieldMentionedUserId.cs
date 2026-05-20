using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by the id of a user mentioned in the message
    /// (<see cref="IStreamMessage.MentionedUsers"/>).
    /// </summary>
    public sealed class MessageFieldMentionedUserId : BaseFieldToFilter
    {
        public override string FieldName => "mentioned_users.id";

        public FieldFilterRule EqualsTo(string userId) => InternalEqualsTo(userId);

        public FieldFilterRule EqualsTo(IStreamUser user) => InternalEqualsTo(user.Id);

        public FieldFilterRule Contains(string userId) => InternalContains(userId);

        public FieldFilterRule Contains(IStreamUser user) => InternalContains(user.Id);

        public FieldFilterRule In(IEnumerable<string> userIds) => InternalIn(userIds);

        public FieldFilterRule In(params string[] userIds) => InternalIn(userIds);

        public FieldFilterRule In(IEnumerable<IStreamUser> users) => InternalIn(users.Select(_ => _.Id));

        public FieldFilterRule In(params IStreamUser[] users) => InternalIn(users.Select(_ => _.Id));
    }
}
