using System.Collections.Generic;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Field names available for <see cref="IStreamChannel"/> query: <see cref="IStreamChatClient.QueryChannelsAsync"/>
    /// </summary>
    public enum ChannelSortFieldName
    {
        LastUpdated,
        LastMessageAt,
        UpdatedAt,
        CreatedAt,
        MemberCount,
        UnreadCount,
        HasUnread
    }
}