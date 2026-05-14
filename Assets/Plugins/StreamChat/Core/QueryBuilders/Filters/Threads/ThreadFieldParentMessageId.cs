using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Threads
{
    /// <summary>
    /// Filter by Thread <c>parent_message_id</c> (the unique id of the thread)
    /// </summary>
    public sealed class ThreadFieldParentMessageId : BaseFieldToFilter
    {
        public override string FieldName => "parent_message_id";

        public FieldFilterRule EqualsTo(string parentMessageId) => InternalEqualsTo(parentMessageId);

        public FieldFilterRule EqualsTo(IStreamMessage parentMessage) => InternalEqualsTo(parentMessage.Id);

        public FieldFilterRule In(IEnumerable<string> parentMessageIds) => InternalIn(parentMessageIds);

        public FieldFilterRule In(params string[] parentMessageIds) => InternalIn(parentMessageIds);

        public FieldFilterRule In(IEnumerable<IStreamMessage> parentMessages)
            => InternalIn(parentMessages.Select(_ => _.Id));

        public FieldFilterRule In(params IStreamMessage[] parentMessages)
            => InternalIn(parentMessages.Select(_ => _.Id));
    }
}
