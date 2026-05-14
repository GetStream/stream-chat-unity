using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    /// <summary>
    /// Internal API for threads operations
    /// </summary>
    internal class InternalThreadsApi : InternalApiClientBase, IInternalThreadsApi
    {
        public InternalThreadsApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory, IStreamChatLowLevelClient lowLevelClient)
            : base(httpClient, serializer, logs, requestUriFactory, lowLevelClient)
        {
        }

        public Task<QueryThreadsResponseInternalDTO> QueryThreadsAsync(QueryThreadsRequestInternalDTO request)
            => Post<QueryThreadsRequestInternalDTO, QueryThreadsResponseInternalDTO>("threads", request);

        public Task<GetThreadResponseInternalDTO> GetThreadAsync(string messageId, int? replyLimit = null,
            int? participantLimit = null, int? memberLimit = null, bool? watch = null)
        {
            var parameters = QueryParameters.Default;
            if (replyLimit.HasValue)
            {
                parameters.Append("reply_limit", replyLimit.Value.ToString());
            }
            if (participantLimit.HasValue)
            {
                parameters.Append("participant_limit", participantLimit.Value.ToString());
            }
            if (memberLimit.HasValue)
            {
                parameters.Append("member_limit", memberLimit.Value.ToString());
            }
            if (watch.HasValue)
            {
                parameters.Append("watch", watch.Value);
            }

            return Get<GetThreadResponseInternalDTO>($"threads/{messageId}", parameters);
        }

        public Task<UpdateThreadPartialResponseInternalDTO> UpdateThreadPartialAsync(string messageId,
            UpdateThreadPartialRequestInternalDTO request)
            => Patch<UpdateThreadPartialRequestInternalDTO, UpdateThreadPartialResponseInternalDTO>($"threads/{messageId}", request);

        public Task<GetRepliesResponseInternalDTO> GetRepliesAsync(string parentId,
            MessagePaginationParamsRequestInternalDTO pagination)
        {
            var parameters = QueryParameters.Default;
            if (pagination != null)
            {
                if (pagination.Limit.HasValue)
                {
                    parameters.Append("limit", pagination.Limit.Value.ToString());
                }
                if (pagination.Offset.HasValue)
                {
                    parameters.Append("offset", pagination.Offset.Value.ToString());
                }
                if (!string.IsNullOrEmpty(pagination.IdGt))
                {
                    parameters.Append("id_gt", pagination.IdGt);
                }
                if (!string.IsNullOrEmpty(pagination.IdGte))
                {
                    parameters.Append("id_gte", pagination.IdGte);
                }
                if (!string.IsNullOrEmpty(pagination.IdLt))
                {
                    parameters.Append("id_lt", pagination.IdLt);
                }
                if (!string.IsNullOrEmpty(pagination.IdLte))
                {
                    parameters.Append("id_lte", pagination.IdLte);
                }
                if (!string.IsNullOrEmpty(pagination.IdAround))
                {
                    parameters.Append("id_around", pagination.IdAround);
                }
                if (pagination.CreatedAtAfter.HasValue)
                {
                    parameters.Append("created_at_after", pagination.CreatedAtAfter.Value.ToString("o"));
                }
                if (pagination.CreatedAtAfterOrEqual.HasValue)
                {
                    parameters.Append("created_at_after_or_equal", pagination.CreatedAtAfterOrEqual.Value.ToString("o"));
                }
                if (pagination.CreatedAtBefore.HasValue)
                {
                    parameters.Append("created_at_before", pagination.CreatedAtBefore.Value.ToString("o"));
                }
                if (pagination.CreatedAtBeforeOrEqual.HasValue)
                {
                    parameters.Append("created_at_before_or_equal", pagination.CreatedAtBeforeOrEqual.Value.ToString("o"));
                }
                if (pagination.CreatedAtAround.HasValue)
                {
                    parameters.Append("created_at_around", pagination.CreatedAtAround.Value.ToString("o"));
                }
            }

            return Get<GetRepliesResponseInternalDTO>($"messages/{parentId}/replies", parameters);
        }
    }
}
