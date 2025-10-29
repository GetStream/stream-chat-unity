using System.Threading.Tasks;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;

namespace StreamChat.Core.LowLevelClient.API
{
    /// <summary>
    /// API client for polls operations
    /// </summary>
    public interface IPollsApi
    {
        /// <summary>
        /// Creates a new poll
        /// </summary>
        Task<PollResponse> CreatePollAsync(CreatePollRequest createPollRequest);

        /// <summary>
        /// Gets a poll by ID
        /// </summary>
        Task<PollResponse> GetPollAsync(string pollId);

        /// <summary>
        /// Updates a poll
        /// </summary>
        Task<PollResponse> UpdatePollAsync(string pollId, UpdatePollRequest updatePollRequest);

        /// <summary>
        /// Partially updates a poll
        /// </summary>
        Task<PollResponse> UpdatePollPartialAsync(string pollId, UpdatePollPartialRequest updatePollPartialRequest);

        /// <summary>
        /// Deletes a poll
        /// </summary>
        Task<ApiResponse> DeletePollAsync(string pollId);

        /// <summary>
        /// Queries polls
        /// </summary>
        Task<QueryPollsResponse> QueryPollsAsync(QueryPollsRequest queryPollsRequest);

        /// <summary>
        /// Creates a poll option
        /// </summary>
        Task<PollOptionResponse> CreatePollOptionAsync(string pollId, CreatePollOptionRequest createPollOptionRequest);

        /// <summary>
        /// Gets a poll option
        /// </summary>
        Task<PollOptionResponse> GetPollOptionAsync(string pollId, string optionId);

        /// <summary>
        /// Updates a poll option
        /// </summary>
        Task<PollOptionResponse> UpdatePollOptionAsync(string pollId, string optionId, UpdatePollOptionRequest updatePollOptionRequest);

        /// <summary>
        /// Deletes a poll option
        /// </summary>
        Task<ApiResponse> DeletePollOptionAsync(string pollId, string optionId);

        /// <summary>
        /// Casts a vote in a poll
        /// </summary>
        Task<PollVoteResponse> CastVoteAsync(string messageId, string pollId, CastPollVoteRequest castPollVoteRequest);

        /// <summary>
        /// Removes a vote from a poll
        /// </summary>
        Task<PollVoteResponse> RemoveVoteAsync(string messageId, string pollId, string voteId);

        /// <summary>
        /// Queries votes for a poll
        /// </summary>
        Task<PollVotesResponse> QueryVotesAsync(string pollId, QueryPollVotesRequest queryPollVotesRequest);
    }
}

