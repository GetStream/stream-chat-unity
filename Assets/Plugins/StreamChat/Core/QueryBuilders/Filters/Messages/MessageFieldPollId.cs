using System.Collections.Generic;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by the id of a poll attached to the message.
    ///
    /// Use <see cref="Exists"/> with <c>true</c> to find any message that has a poll attached,
    /// or pass a specific poll id to find the message that hosts a known poll.
    /// </summary>
    public sealed class MessageFieldPollId : BaseFieldToFilter
    {
        public override string FieldName => "poll_id";

        public FieldFilterRule EqualsTo(string pollId) => InternalEqualsTo(pollId);

        public FieldFilterRule In(IEnumerable<string> pollIds) => InternalIn(pollIds);

        public FieldFilterRule In(params string[] pollIds) => InternalIn(pollIds);

        /// <summary>
        /// When <c>true</c>, returns only messages that have a poll attached.
        /// When <c>false</c>, returns only messages without a poll.
        /// </summary>
        public FieldFilterRule Exists(bool exists) => InternalExists(exists);
    }
}
