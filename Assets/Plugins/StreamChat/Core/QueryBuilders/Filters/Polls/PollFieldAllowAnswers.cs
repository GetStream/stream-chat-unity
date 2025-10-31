namespace StreamChat.Core.QueryBuilders.Filters.Polls
{
    /// <summary>
    /// Filter by Poll AllowAnswers
    /// </summary>
    public sealed class PollFieldAllowAnswers : BaseFieldToFilter
    {
        public override string FieldName => "allow_answers";

        /// <summary>
        /// Return only polls where AllowAnswers is EQUAL to provided value
        /// </summary>
        public FieldFilterRule EqualsTo(bool allowAnswers) => InternalEqualsTo(allowAnswers);
    }
}

