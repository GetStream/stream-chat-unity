using System.Collections.Generic;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by the type of a reaction on the message (e.g. <c>like</c>, <c>love</c>, <c>fire</c>).
    /// Matches the latest reactions tracked server-side.
    /// </summary>
    public sealed class MessageFieldReactionType : BaseFieldToFilter
    {
        public override string FieldName => "latest_reactions.type";

        public FieldFilterRule EqualsTo(string reactionType) => InternalEqualsTo(reactionType);

        public FieldFilterRule Contains(string reactionType) => InternalContains(reactionType);

        public FieldFilterRule In(IEnumerable<string> reactionTypes) => InternalIn(reactionTypes);

        public FieldFilterRule In(params string[] reactionTypes) => InternalIn(reactionTypes);
    }
}
