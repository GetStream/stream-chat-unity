using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Polls
{
    /// <summary>
    /// Filter by Poll Id
    /// </summary>
    public sealed class PollFieldId : BaseFieldToFilter
    {
        public override string FieldName => "id";

        /// <summary>
        /// Return only polls where Id is EQUAL to provided poll Id
        /// </summary>
        public FieldFilterRule EqualsTo(string pollId) => InternalEqualsTo(pollId);

        /// <summary>
        /// Return only polls where Id is EQUAL to ANY of provided poll Ids
        /// </summary>
        public FieldFilterRule In(IEnumerable<string> pollIds) => InternalIn(pollIds);

        /// <summary>
        /// Return only polls where Id is EQUAL to ANY of provided poll Ids
        /// </summary>
        public FieldFilterRule In(params string[] pollIds) => InternalIn(pollIds);

        /// <summary>
        /// Return only polls where Id is EQUAL to ANY of the provided polls Id
        /// </summary>
        public FieldFilterRule In(IEnumerable<IStreamPoll> polls)
            => InternalIn(polls.Select(_ => _.Id));

        /// <summary>
        /// Return only polls where Id is EQUAL to ANY of the provided polls Id
        /// </summary>
        public FieldFilterRule In(params IStreamPoll[] polls)
            => InternalIn(polls.Select(_ => _.Id));
    }
}

