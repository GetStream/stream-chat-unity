using System;

namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Sort object for Poll query
    /// </summary>
    public sealed class PollsSortObject : QuerySort<PollsSortObject, PollSortFieldName>
    {
        protected override PollsSortObject Instance => this;

        protected override string ToUnderlyingFieldName(PollSortFieldName fieldName)
        {
            switch (fieldName)
            {
                case PollSortFieldName.CreatedAt: return "created_at";
                case PollSortFieldName.UpdatedAt: return "updated_at";
                case PollSortFieldName.Id: return "id";
                case PollSortFieldName.Name: return "name";
                case PollSortFieldName.VoteCount: return "vote_count";
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldName), fieldName, null);
            }
        }
    }

    /// <summary>
    /// Sort field names for polls
    /// </summary>
    public enum PollSortFieldName
    {
        CreatedAt,
        UpdatedAt,
        Id,
        Name,
        VoteCount
    }
}

