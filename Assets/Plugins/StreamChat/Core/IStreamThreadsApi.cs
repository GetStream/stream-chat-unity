using System.Threading.Tasks;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core
{
    /// <summary>
    /// Threads API for retrieving and querying threads.
    /// A thread groups replies to a parent <see cref="IStreamMessage"/> in a <see cref="IStreamChannel"/>.
    /// </summary>
    public interface IStreamThreadsApi
    {
        /// <summary>
        /// Get a thread by its parent message id
        /// </summary>
        /// <param name="parentMessageId">The id of the parent message of the thread</param>
        /// <param name="replyLimit">[Optional] Number of replies to fetch</param>
        /// <param name="participantLimit">[Optional] Number of participants to fetch</param>
        /// <param name="memberLimit">[Optional] Number of channel members to include</param>
        /// <param name="watch">[Optional] Whether to start watching the channel this thread belongs to. Defaults to true.</param>
        /// <returns>The requested thread</returns>
        Task<IStreamThread> GetThreadAsync(string parentMessageId,
            int? replyLimit = null,
            int? participantLimit = null,
            int? memberLimit = null,
            bool watch = true);

        /// <summary>
        /// Query threads with optional filters and sorting
        /// </summary>
        /// <param name="request">Query request</param>
        Task<StreamQueryThreadsResponse> QueryThreadsAsync(StreamQueryThreadsRequest request);
    }
}
