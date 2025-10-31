namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Factory for Poll query sort object building
    /// </summary>
    public static class PollSort
    {
        /// <summary>
        /// Sort in ascending order meaning from lowest to highest value of the specified field
        /// </summary>
        /// <param name="fieldName">Field name to sort by</param>
        public static PollsSortObject OrderByAscending(PollSortFieldName fieldName)
        {
            var instance = new PollsSortObject();
            instance.OrderByAscending(fieldName);
            return instance;
        }

        /// <summary>
        /// Sort in descending order meaning from highest to lowest value of the specified field
        /// </summary>
        /// <param name="fieldName">Field name to sort by</param>
        public static PollsSortObject OrderByDescending(PollSortFieldName fieldName)
        {
            var instance = new PollsSortObject();
            instance.OrderByDescending(fieldName);
            return instance;
        }

        /// <summary>
        /// Then sort in ascending order meaning from lowest to highest value of the specified field
        /// </summary>
        /// <param name="sort">Current sort object</param>
        /// <param name="fieldName">Field name to sort by</param>
        public static PollsSortObject ThenByAscending(this PollsSortObject sort, PollSortFieldName fieldName)
            => sort.OrderByAscending(fieldName);

        /// <summary>
        /// Then sort in descending order meaning from highest to lowest value of the specified field
        /// </summary>
        /// <param name="sort">Current sort object</param>
        /// <param name="fieldName">Field name to sort by</param>
        public static PollsSortObject ThenByDescending(this PollsSortObject sort, PollSortFieldName fieldName)
            => sort.OrderByDescending(fieldName);
    }
}

