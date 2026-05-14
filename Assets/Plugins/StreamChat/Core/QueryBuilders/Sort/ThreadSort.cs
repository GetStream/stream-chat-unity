namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Factory for Threads query sort object building
    /// </summary>
    public static class ThreadSort
    {
        /// <summary>
        /// Sort in ascending order (lowest to highest) by the specified field
        /// </summary>
        public static ThreadsSortObject OrderByAscending(ThreadSortFieldName fieldName)
        {
            var instance = new ThreadsSortObject();
            instance.OrderByAscending(fieldName);
            return instance;
        }

        /// <summary>
        /// Sort in descending order (highest to lowest) by the specified field
        /// </summary>
        public static ThreadsSortObject OrderByDescending(ThreadSortFieldName fieldName)
        {
            var instance = new ThreadsSortObject();
            instance.OrderByDescending(fieldName);
            return instance;
        }

        /// <summary>
        /// Then sort in ascending order (lowest to highest) by the specified field
        /// </summary>
        public static ThreadsSortObject ThenByAscending(this ThreadsSortObject sort, ThreadSortFieldName fieldName)
            => sort.OrderByAscending(fieldName);

        /// <summary>
        /// Then sort in descending order (highest to lowest) by the specified field
        /// </summary>
        public static ThreadsSortObject ThenByDescending(this ThreadsSortObject sort, ThreadSortFieldName fieldName)
            => sort.OrderByDescending(fieldName);
    }
}
