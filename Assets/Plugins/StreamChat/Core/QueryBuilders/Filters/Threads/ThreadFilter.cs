namespace StreamChat.Core.QueryBuilders.Filters.Threads
{
    /// <summary>
    /// Filters for Thread query
    /// </summary>
    public static class ThreadFilter
    {
        /// <inheritdoc cref="ThreadFieldChannelCid"/>
        public static ThreadFieldChannelCid ChannelCid { get; } = new ThreadFieldChannelCid();

        /// <inheritdoc cref="ThreadFieldParentMessageId"/>
        public static ThreadFieldParentMessageId ParentMessageId { get; } = new ThreadFieldParentMessageId();

        /// <inheritdoc cref="ThreadFieldCreatedByUserId"/>
        public static ThreadFieldCreatedByUserId CreatedByUserId { get; } = new ThreadFieldCreatedByUserId();

        /// <inheritdoc cref="ThreadFieldCreatedAt"/>
        public static ThreadFieldCreatedAt CreatedAt { get; } = new ThreadFieldCreatedAt();

        /// <inheritdoc cref="ThreadFieldUpdatedAt"/>
        public static ThreadFieldUpdatedAt UpdatedAt { get; } = new ThreadFieldUpdatedAt();

        /// <inheritdoc cref="ThreadFieldLastMessageAt"/>
        public static ThreadFieldLastMessageAt LastMessageAt { get; } = new ThreadFieldLastMessageAt();

        /// <inheritdoc cref="ThreadFieldChannelTeam"/>
        public static ThreadFieldChannelTeam ChannelTeam { get; } = new ThreadFieldChannelTeam();

        /// <inheritdoc cref="ThreadFieldChannelDisabled"/>
        public static ThreadFieldChannelDisabled ChannelDisabled { get; } = new ThreadFieldChannelDisabled();
    }
}
