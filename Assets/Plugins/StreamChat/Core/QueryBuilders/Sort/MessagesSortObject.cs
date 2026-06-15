using System;

namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Sort object for <see cref="IStreamChatClient.SearchMessagesAsync"/>.
    /// </summary>
    public sealed class MessagesSortObject : QuerySort<MessagesSortObject, MessageSortFieldName>
    {
        protected override MessagesSortObject Instance => this;

        protected override string ToUnderlyingFieldName(MessageSortFieldName fieldName)
        {
            switch (fieldName)
            {
                case MessageSortFieldName.CreatedAt: return "created_at";
                case MessageSortFieldName.UpdatedAt: return "updated_at";
                case MessageSortFieldName.Relevance: return "relevance";
                case MessageSortFieldName.Id: return "id";
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldName), fieldName, null);
            }
        }
    }

    /// <summary>
    /// Sort field names for <see cref="IStreamChatClient.SearchMessagesAsync"/>.
    /// </summary>
    public enum MessageSortFieldName
    {
        CreatedAt,
        UpdatedAt,
        Relevance,
        Id,
    }
}
