using System.Collections.Generic;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Responses
{
    /// <summary>
    /// Response from <see cref="IStreamThreadsApi.QueryThreadsAsync"/>
    /// </summary>
    public class StreamQueryThreadsResponse
    {
        /// <summary>
        /// Threads matching the query (already cached and tracked)
        /// </summary>
        public IReadOnlyList<IStreamThread> Threads { get; internal set; }

        /// <summary>
        /// Pagination cursor for the next page (null if no more pages)
        /// </summary>
        public string Next { get; internal set; }

        /// <summary>
        /// Pagination cursor for the previous page
        /// </summary>
        public string Prev { get; internal set; }
    }
}
