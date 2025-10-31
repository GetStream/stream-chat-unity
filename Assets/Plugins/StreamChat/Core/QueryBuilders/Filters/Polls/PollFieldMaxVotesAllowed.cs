namespace StreamChat.Core.QueryBuilders.Filters.Polls
{
    /// <summary>
    /// Filter by Poll MaxVotesAllowed
    /// </summary>
    public sealed class PollFieldMaxVotesAllowed : BaseFieldToFilter
    {
        public override string FieldName => "max_votes_allowed";

        /// <summary>
        /// Return only polls where MaxVotesAllowed is EQUAL to provided value
        /// </summary>
        public FieldFilterRule EqualsTo(int maxVotes) => InternalEqualsTo(maxVotes);

        /// <summary>
        /// Return only polls where MaxVotesAllowed is NOT EQUAL to provided value
        /// </summary>
        public FieldFilterRule NotEquals(int maxVotes) => new FieldFilterRule(FieldName, QueryOperatorType.NotEquals, maxVotes);

        /// <summary>
        /// Return only polls where MaxVotesAllowed is GREATER THAN provided value
        /// </summary>
        public FieldFilterRule GreaterThan(int maxVotes) => InternalGreaterThan(maxVotes);

        /// <summary>
        /// Return only polls where MaxVotesAllowed is GREATER THAN OR EQUAL to provided value
        /// </summary>
        public FieldFilterRule GreaterThanOrEquals(int maxVotes) => InternalGreaterThanOrEquals(maxVotes);

        /// <summary>
        /// Return only polls where MaxVotesAllowed is LESS THAN provided value
        /// </summary>
        public FieldFilterRule LessThan(int maxVotes) => InternalLessThan(maxVotes);

        /// <summary>
        /// Return only polls where MaxVotesAllowed is LESS THAN OR EQUAL to provided value
        /// </summary>
        public FieldFilterRule LessThanOrEquals(int maxVotes) => InternalLessThanOrEquals(maxVotes);
    }
}

