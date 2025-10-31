using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    /// <summary>
    /// Internal API for polls operations
    /// </summary>
    internal interface IInternalPollsApi
    {
        /// <summary>
        /// Create a new poll
        /// </summary>
        Task<PollResponseInternalDTO> CreatePollAsync(CreatePollRequestInternalDTO createPollRequest);

        /// <summary>
        /// Get a poll by ID
        /// </summary>
        Task<PollResponseInternalDTO> GetPollAsync(string pollId);

        /// <summary>
        /// Update a poll
        /// </summary>
        Task<PollResponseInternalDTO> UpdatePollAsync(UpdatePollRequestInternalDTO updatePollRequest);

        /// <summary>
        /// Partial update a poll
        /// </summary>
        Task<PollResponseInternalDTO> UpdatePollPartialAsync(string pollId, UpdatePollPartialRequestInternalDTO updatePollPartialRequest);

        /// <summary>
        /// Delete a poll
        /// </summary>
        Task<ResponseInternalDTO> DeletePollAsync(string pollId);

        /// <summary>
        /// Query polls
        /// </summary>
        Task<QueryPollsResponseInternalDTO> QueryPollsAsync(QueryPollsRequestInternalDTO queryPollsRequest);

        /// <summary>
        /// Create a poll option
        /// </summary>
        Task<PollOptionResponseInternalDTO> CreatePollOptionAsync(string pollId, CreatePollOptionRequestInternalDTO createPollOptionRequest);

        /// <summary>
        /// Get a poll option
        /// </summary>
        Task<PollOptionResponseInternalDTO> GetPollOptionAsync(string pollId, string optionId);

        /// <summary>
        /// Update a poll option
        /// </summary>
        Task<PollOptionResponseInternalDTO> UpdatePollOptionAsync(string pollId, string optionId, UpdatePollOptionRequestInternalDTO updatePollOptionRequest);

        /// <summary>
        /// Delete a poll option
        /// </summary>
        Task<ResponseInternalDTO> DeletePollOptionAsync(string pollId, string optionId);

        /// <summary>
        /// Cast a vote
        /// </summary>
        Task<PollVoteResponseInternalDTO> CastVoteAsync(string messageId, string pollId, CastPollVoteRequestInternalDTO castPollVoteRequest);

        /// <summary>
        /// Remove a vote
        /// </summary>
        Task<PollVoteResponseInternalDTO> RemoveVoteAsync(string messageId, string pollId, string voteId);

        /// <summary>
        /// Query votes
        /// </summary>
        Task<PollVotesResponseInternalDTO> QueryVotesAsync(string pollId, QueryPollVotesRequestInternalDTO queryPollVotesRequest);
    }
}

