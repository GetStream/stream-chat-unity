using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    /// <summary>
    /// Internal API for threads operations
    /// </summary>
    internal interface IInternalThreadsApi
    {
        /// <summary>
        /// Query threads with optional filters and sorting
        /// </summary>
        Task<QueryThreadsResponseInternalDTO> QueryThreadsAsync(QueryThreadsRequestInternalDTO request);

        /// <summary>
        /// Get a single thread state by parent message id
        /// </summary>
        Task<GetThreadResponseInternalDTO> GetThreadAsync(string messageId, int? replyLimit = null,
            int? participantLimit = null, int? memberLimit = null, bool? watch = null);

        /// <summary>
        /// Partially update a thread (set/unset fields, custom data, title)
        /// </summary>
        Task<UpdateThreadPartialResponseInternalDTO> UpdateThreadPartialAsync(string messageId,
            UpdateThreadPartialRequestInternalDTO request);

        /// <summary>
        /// Get the replies of a parent message (paginated)
        /// </summary>
        Task<GetRepliesResponseInternalDTO> GetRepliesAsync(string parentId,
            MessagePaginationParamsRequestInternalDTO pagination);
    }
}
