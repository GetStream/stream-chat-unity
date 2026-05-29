using System.Collections.Generic;

namespace StreamChat.Core.Responses
{
    /// <summary>
    /// Response from <see cref="IStreamChatClient.SearchMessagesAsync"/>.
    /// </summary>
    public sealed class StreamSearchMessagesResponse
    {
        /// <summary>
        /// The matching messages.
        /// </summary>
        public IReadOnlyList<StreamSearchMessageResult> Results { get; internal set; }

        /// <summary>
        /// Cursor for the next page; <c>null</c> if there are no more pages.
        /// Pass this value as <see cref="Requests.StreamSearchMessagesRequest.Next"/> to retrieve the next page.
        /// </summary>
        public string Next { get; internal set; }

        /// <summary>
        /// Cursor for the previous page; <c>null</c> on the first page.
        /// </summary>
        public string Previous { get; internal set; }

        /// <summary>
        /// Human-readable request duration as reported by the server.
        /// </summary>
        public string Duration { get; internal set; }

        /// <summary>
        /// Optional warning emitted by the server about the search result set
        /// (e.g. truncated channel scope).
        /// </summary>
        public StreamSearchWarning ResultsWarning { get; internal set; }
    }
}
