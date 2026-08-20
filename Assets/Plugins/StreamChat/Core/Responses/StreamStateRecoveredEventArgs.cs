using System;
using System.Collections.Generic;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Responses
{
    /// <summary>
    /// Payload for <see cref="IStreamChatClient.StateRecovered"/>.
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
        /// Channels whose state was refreshed and whose watch was re-established. Their
        /// <see cref="IStreamChannel.Messages"/> and other collections are up to date as of the
        /// moment this event is raised.
        ///
        /// Note that the recovery query returns the channel's latest page of messages and merges it
        /// into what was already loaded. If more messages arrived during the outage than fit in one
        /// page, the list contains the pre-disconnect messages followed by the latest page with a
        /// hole in between, and <see cref="IStreamChannel.LoadOlderMessagesAsync"/> cannot reach into
        /// that hole because it pages back from the oldest loaded message.
        /// </summary>
        public IReadOnlyList<IStreamChannel> Channels { get; }

        /// <summary>
        /// Channels that were being watched before the disconnect but could not be recovered - the
        /// server no longer returns them (deleted, or the local user lost access while offline), or
        /// every attempt to re-query them failed. Their local state is still stale and they are no
        /// longer watched, so they will not receive realtime updates.
        ///
        /// Empty on a fully successful recovery. Use it to tear down or flag the corresponding UI
        /// rather than leaving it silently frozen.
        /// </summary>
        public IReadOnlyList<string> UnrecoveredChannelCids { get; }

        /// <summary>
        /// <c>true</c> when every channel that was being watched before the disconnect was recovered.
        /// </summary>
        public bool IsComplete => UnrecoveredChannelCids.Count == 0;
    }
}
