using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core
{
    /// <summary>
    /// Polls API for creating, retrieving, and querying polls
    /// </summary>
    public interface IStreamPollsApi
    {
        /// <summary>
        /// Create a new poll
        /// </summary>
        /// <param name="createRequest">Request with poll creation data</param>
        /// <returns>The created poll</returns>
        Task<IStreamPoll> CreatePollAsync(StreamCreatePollRequest createRequest);

        /// <summary>
        /// Get a poll by ID
        /// </summary>
        /// <param name="pollId">The poll ID</param>
        /// <returns>The poll with the specified ID</returns>
        Task<IStreamPoll> GetPollAsync(string pollId);

        /// <summary>
        /// Query polls based on filters
        /// </summary>
        /// <param name="queryRequest">Request with query filters and parameters</param>
        /// <returns>List of polls matching the query</returns>
        Task<IEnumerable<IStreamPoll>> QueryPollsAsync(StreamQueryPollsRequest queryRequest);

        /// <summary>
        /// Delete a poll by ID
        /// </summary>
        /// <param name="pollId">The poll ID to delete</param>
        Task DeletePollAsync(string pollId);
    }
}

