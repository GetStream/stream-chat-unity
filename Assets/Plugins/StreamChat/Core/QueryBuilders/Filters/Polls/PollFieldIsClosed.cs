namespace StreamChat.Core.QueryBuilders.Filters.Polls
{
    /// <summary>
    /// Filter by Poll IsClosed status
    /// </summary>
    public sealed class PollFieldIsClosed : BaseFieldToFilter
    {
        public override string FieldName => "is_closed";

        /// <summary>
        /// Return only polls where IsClosed status is EQUAL to the provided value
        /// </summary>
        public FieldFilterRule EqualsTo(bool isClosed) => InternalEqualsTo(isClosed);
    }
}

