using System;

namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Sort object for Threads query
    /// </summary>
    public sealed class ThreadsSortObject : QuerySort<ThreadsSortObject, ThreadSortFieldName>
    {
        protected override ThreadsSortObject Instance => this;

        protected override string ToUnderlyingFieldName(ThreadSortFieldName fieldName)
        {
            switch (fieldName)
            {
                case ThreadSortFieldName.ActiveParticipantCount: return "active_participant_count";
                case ThreadSortFieldName.CreatedAt: return "created_at";
                case ThreadSortFieldName.LastMessageAt: return "last_message_at";
                case ThreadSortFieldName.ParentMessageId: return "parent_message_id";
                case ThreadSortFieldName.ParticipantCount: return "participant_count";
                case ThreadSortFieldName.ReplyCount: return "reply_count";
                case ThreadSortFieldName.UpdatedAt: return "updated_at";
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldName), fieldName, null);
            }
        }
    }

    /// <summary>
    /// Sort field names for threads
    /// </summary>
    public enum ThreadSortFieldName
    {
        ActiveParticipantCount,
        CreatedAt,
        LastMessageAt,
        ParentMessageId,
        ParticipantCount,
        ReplyCount,
        UpdatedAt
    }
}
