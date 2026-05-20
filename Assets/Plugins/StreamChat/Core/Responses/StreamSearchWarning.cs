using System.Collections.Generic;

namespace StreamChat.Core.Responses
{
    /// <summary>
    /// Warning emitted by the server alongside a <see cref="IStreamChatClient.SearchMessagesAsync"/>
    /// response (e.g. when the searched-channel scope was truncated).
    /// </summary>
    public sealed class StreamSearchWarning
    {
        /// <summary>
        /// Numeric warning code as reported by the server, or <c>null</c> if not provided.
        /// </summary>
        public int? Code { get; internal set; }

        /// <summary>
        /// Human-readable description of the warning.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Number of channels included in the searched scope, when reported by the server.
        /// </summary>
        public int? ChannelSearchCount { get; internal set; }

        /// <summary>
        /// Cids of the channels that were searched, when reported by the server.
        /// </summary>
        public IReadOnlyList<string> ChannelIds { get; internal set; }
    }
}
