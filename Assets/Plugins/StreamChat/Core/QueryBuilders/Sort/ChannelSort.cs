using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Factory for <see cref="IStreamChannel"/> query <see cref="IStreamChatClient.QueryChannelsAsync"/> sort object building
    /// </summary>
    public static class ChannelSort
    {
        /// <summary>
        /// Sort in ascending order meaning from lowest to highest value of the specified field
        /// </summary>
        /// <param name="fieldName">Field name to sort by</param>
        public static ChannelSortObject OrderByAscending(ChannelSortFieldName fieldName)
        {
            var instance = new ChannelSortObject();
            instance.OrderByAscending(fieldName);
            return instance;
        }

        /// <summary>
        /// Sort in descending order meaning from highest to lowest value of the specified field
        /// </summary>
        /// <param name="fieldName">Field name to sort by</param>
        public static ChannelSortObject OrderByDescending(ChannelSortFieldName fieldName)
        {
            var instance = new ChannelSortObject();
            instance.OrderByDescending(fieldName);
            return instance;
        }

        /// <summary>
        /// Sort in descending order meaning from highest to lowest value of the specified field
        /// </summary>
        /// <param name="fieldName">Field name to sort by</param>
        public static ChannelSortObject ThenByAscending(this ChannelSortObject sort, ChannelSortFieldName fieldName)
            => sort.OrderByAscending(fieldName);

        /// <summary>
        /// Sort in descending order meaning from highest to lowest value of the specified field
        /// </summary>
        /// <param name="fieldName">Field name to sort by</param>
        public static ChannelSortObject ThenByDescending(this ChannelSortObject sort, ChannelSortFieldName fieldName)
            => sort.OrderByDescending(fieldName);
    }
}