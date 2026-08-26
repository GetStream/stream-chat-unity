using System;

namespace StreamChat.Core.LowLevelClient
{
    /// <summary>
    /// Result of applying one <c>/sync</c> history batch.
    /// </summary>
    internal sealed class HistorySyncApplyResult
    {
        /// <summary>
        /// <c>created_at</c> of the newest event that applied, or <c>null</c>.
        /// The <c>/sync</c> <c>last_sync_at</c> cursor advances to this.
        /// </summary>
        public DateTimeOffset? MaxAppliedCreatedAt { get; set; }

        /// <summary>
        /// Events that threw while applying. Skipped, not retried in this batch.
        /// </summary>
        public int FailedEventCount { get; set; }
    }
}
