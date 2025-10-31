namespace StreamChat.Core.QueryBuilders.Filters.Polls
{
    /// <summary>
    /// Filter by Poll VotingVisibility
    /// </summary>
    public sealed class PollFieldVotingVisibility : BaseFieldToFilter
    {
        public override string FieldName => "voting_visibility";

        /// <summary>
        /// Return only polls where VotingVisibility is EQUAL to provided value
        /// </summary>
        public FieldFilterRule EqualsTo(string votingVisibility) => InternalEqualsTo(votingVisibility);
    }
}

