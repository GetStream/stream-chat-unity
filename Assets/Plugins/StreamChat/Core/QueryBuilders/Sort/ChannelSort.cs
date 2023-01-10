using System.Collections.Generic;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Factory for creating for <see cref="IStreamChannel"/> query <see cref="IStreamChatClient.QueryChannelsAsync(IDictionary{string,object}, ChannelSortObject, int, int)"/> sort object
    /// </summary>
    public static class ChannelSort
    {
        public static ChannelSortObject OrderByAscending(ChannelSortFieldName fieldName)
        {
            var instance = new ChannelSortObject();
            instance.OrderByAscending(fieldName);
            return instance;
        }

        public static ChannelSortObject OrderByDescending(ChannelSortFieldName fieldName)
        {
            var instance = new ChannelSortObject();
            instance.OrderByDescending(fieldName);
            return instance;
        }

        public static ChannelSortObject ThenByAscending(this ChannelSortObject sort, ChannelSortFieldName fieldName)
            => sort.OrderByAscending(fieldName);

        public static ChannelSortObject ThenByDescending(this ChannelSortObject sort, ChannelSortFieldName fieldName)
            => sort.OrderByDescending(fieldName);
    }
}