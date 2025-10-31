namespace StreamChat.Core.QueryBuilders.Filters.Polls
{
    /// <summary>
    /// Filter by Poll AllowUserSuggestedOptions
    /// </summary>
    public sealed class PollFieldAllowUserSuggestedOptions : BaseFieldToFilter
    {
        public override string FieldName => "allow_user_suggested_options";

        /// <summary>
        /// Return only polls where AllowUserSuggestedOptions is EQUAL to provided value
        /// </summary>
        public FieldFilterRule EqualsTo(bool allowUserSuggestedOptions) => InternalEqualsTo(allowUserSuggestedOptions);
    }
}

