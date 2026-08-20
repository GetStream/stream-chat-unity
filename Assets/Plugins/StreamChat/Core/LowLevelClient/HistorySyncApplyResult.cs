using System;

namespace StreamChat.Core.LowLevelClient
{
    /// <summary>
    /// Outcome of one silent <c>/sync</c> history batch. See
    /// <see cref="StreamChatLowLevelClient.ApplyHistoryEvents"/>.
    /// </summary>
    internal sealed class HistorySyncApplyResult
    {
        /// <summary>
        /// <c>created_at</c> of the newest event that was applied successfully, or <c>null</c> when
        /// nothing was applied. This is what the <c>/sync</c> watermark advances to.
        /// </summary>
        public DateTimeOffset? MaxAppliedCreatedAt { get; set; }

        /// <summary>
        /// Events that threw while being applied. They are skipped, not retried within the batch.
        /// </summary>
        public int FailedEventCount { get; set; }
    }
}
