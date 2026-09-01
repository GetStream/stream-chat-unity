using System;
using System.Collections.Generic;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Responses
{
    /// <summary>
    /// Data for <see cref="IStreamChatClient.StateRecovered"/>.
    /// </summary>
    public sealed class StreamStateRecoveredEventArgs
    {
        public StreamStateRecoveredEventArgs(IReadOnlyList<IStreamChannel> channels,
            IReadOnlyList<string> unrecoveredChannelCids)
        {
            Channels = channels ?? Array.Empty<IStreamChannel>();
            UnrecoveredChannelCids = unrecoveredChannelCids ?? Array.Empty<string>();
        }

        /// <summary>
        /// Channels that were refreshed and are watched again.
        /// Their <see cref="IStreamChannel.Messages"/> and other collections are up to date when this event is raised.
        ///
        /// Recovery adds the newest messages to what was already loaded.
        /// If many messages arrived while you were disconnected, there can be a gap in the list.
        /// <see cref="IStreamChannel.LoadOlderMessagesAsync"/> cannot fill that gap,
        /// because it loads from the oldest loaded message.
        /// </summary>
        public IReadOnlyList<IStreamChannel> Channels { get; }

        /// <summary>
        /// Channels that were watched before the disconnect but could not be recovered.
        /// The server no longer returns them (deleted, or the user lost access), or the client could not refresh them.
        /// Their local state is still stale and they are not watched.
        ///
        /// Empty when recovery fully succeeds. Use this to close or mark the related UI.
        /// </summary>
        public IReadOnlyList<string> UnrecoveredChannelCids { get; }

        /// <summary>
        /// True when every watched channel from before the disconnect was recovered.
        /// </summary>
        public bool IsComplete => UnrecoveredChannelCids.Count == 0;
    }
}
