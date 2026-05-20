namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Factory for <see cref="IStreamChatClient.SearchMessagesAsync"/> sort object building.
    /// </summary>
    /// <remarks>
    /// Note: the server forbids combining a sort with a non-zero <c>offset</c>. To paginate
    /// sorted results use the <c>Next</c> cursor returned by the previous response.
    /// </remarks>
    public static class MessagesSort
    {
        /// <summary>
        /// Sort in ascending order (lowest to highest) by the specified field.
        /// </summary>
        public static MessagesSortObject OrderByAscending(MessageSortFieldName fieldName)
        {
            var instance = new MessagesSortObject();
            instance.OrderByAscending(fieldName);
            return instance;
        }

        /// <summary>
        /// Sort in descending order (highest to lowest) by the specified field.
        /// </summary>
        public static MessagesSortObject OrderByDescending(MessageSortFieldName fieldName)
        {
            var instance = new MessagesSortObject();
            instance.OrderByDescending(fieldName);
            return instance;
        }

        /// <summary>
        /// Then sort in ascending order (lowest to highest) by the specified field.
        /// </summary>
        public static MessagesSortObject ThenByAscending(this MessagesSortObject sort, MessageSortFieldName fieldName)
            => sort.OrderByAscending(fieldName);

        /// <summary>
        /// Then sort in descending order (highest to lowest) by the specified field.
        /// </summary>
        public static MessagesSortObject ThenByDescending(this MessagesSortObject sort, MessageSortFieldName fieldName)
            => sort.OrderByDescending(fieldName);
    }
}
