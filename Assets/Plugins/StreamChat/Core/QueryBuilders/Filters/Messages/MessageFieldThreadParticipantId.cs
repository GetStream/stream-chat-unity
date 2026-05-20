using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by the id of a user participating in the thread the message belongs to.
    /// </summary>
    public sealed class MessageFieldThreadParticipantId : BaseFieldToFilter
    {
        public override string FieldName => "thread_participants.id";

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
