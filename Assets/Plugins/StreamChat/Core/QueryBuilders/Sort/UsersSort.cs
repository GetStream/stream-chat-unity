using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Factory for <see cref="IStreamUser"/> query <see cref="IStreamChatClient.QueryUsersAsync"/> sort object building
    /// </summary>
    public static class UsersSort
    {
        /// <summary>
        /// Sort in ascending order meaning from lowest to highest value of the specified field
        /// </summary>
        /// <param name="fieldName">Field name to sort by</param>
        public static UsersSortObject OrderByAscending(UserSortField fieldName)
        {
            var instance = new UsersSortObject();
            instance.OrderByAscending(fieldName);
            return instance;
        }

        /// <summary>
        /// Sort in descending order meaning from highest to lowest value of the specified field
        /// </summary>
        /// <param name="fieldName">Field name to sort by</param>
        public static UsersSortObject OrderByDescending(UserSortField fieldName)
        {
            var instance = new UsersSortObject();
            instance.OrderByDescending(fieldName);
            return instance;
        }

        /// <summary>
        /// Sort in descending order meaning from highest to lowest value of the specified field
        /// </summary>
        /// <param name="fieldName">Field name to sort by</param>
        public static UsersSortObject ThenByAscending(this UsersSortObject sort, UserSortField fieldName)
            => sort.OrderByAscending(fieldName);

        /// <summary>
        /// Sort in descending order meaning from highest to lowest value of the specified field
        /// </summary>
        /// <param name="fieldName">Field name to sort by</param>
        public static UsersSortObject ThenByDescending(this UsersSortObject sort, UserSortField fieldName)
            => sort.OrderByDescending(fieldName);
    }
}