using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Fields that you can use to sort <see cref="IStreamChannel"/> query results when using <see cref="IStreamChatClient.QueryChannelsAsync"/>
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