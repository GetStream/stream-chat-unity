namespace StreamChat.Core.QueryBuilders.Filters.Polls
{
    /// <summary>
    /// Filters for Poll query
    /// </summary>
    public static class PollFilter
    {
        /// <inheritdoc cref="PollFieldId"/>
        public static PollFieldId Id { get; } = new PollFieldId();

        /// <inheritdoc cref="PollFieldName"/>
        public static PollFieldName Name { get; } = new PollFieldName();

        /// <inheritdoc cref="PollFieldVotingVisibility"/>
        public static PollFieldVotingVisibility VotingVisibility { get; } = new PollFieldVotingVisibility();

        /// <inheritdoc cref="PollFieldMaxVotesAllowed"/>
        public static PollFieldMaxVotesAllowed MaxVotesAllowed { get; } = new PollFieldMaxVotesAllowed();

        /// <inheritdoc cref="PollFieldAllowUserSuggestedOptions"/>
        public static PollFieldAllowUserSuggestedOptions AllowUserSuggestedOptions { get; } = new PollFieldAllowUserSuggestedOptions();

        /// <inheritdoc cref="PollFieldAllowAnswers"/>
        public static PollFieldAllowAnswers AllowAnswers { get; } = new PollFieldAllowAnswers();

        /// <inheritdoc cref="PollFieldIsClosed"/>
        public static PollFieldIsClosed IsClosed { get; } = new PollFieldIsClosed();

        /// <inheritdoc cref="PollFieldCreatedAt"/>
        public static PollFieldCreatedAt CreatedAt { get; } = new PollFieldCreatedAt();

        /// <inheritdoc cref="PollFieldUpdatedAt"/>
        public static PollFieldUpdatedAt UpdatedAt { get; } = new PollFieldUpdatedAt();

        /// <inheritdoc cref="PollFieldCreatedById"/>
        public static PollFieldCreatedById CreatedById { get; } = new PollFieldCreatedById();
    }
}

