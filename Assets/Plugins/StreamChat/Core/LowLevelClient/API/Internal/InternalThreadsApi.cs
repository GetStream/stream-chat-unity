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
            => Get<MessagePaginationParamsRequestInternalDTO, GetRepliesResponseInternalDTO>(
                $"messages/{parentId}/replies", pagination);
    }
}
