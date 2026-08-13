using System;

namespace StreamChat.Core.Configs
{
    /// <summary>
    /// Limits how many messages a channel keeps in the local cache.
    /// Assign to <see cref="IStreamClientConfig.DefaultMessageCacheWindow"/> or
    /// <see cref="StatefulModels.IStreamChannel.OverrideMessageCacheWindow"/>.
    /// Trimming runs in batches of <see cref="DiscardBatchSize"/> once the count exceeds
    /// <see cref="MaxMessages"/>. Nothing is ever removed while
    /// <see cref="StatefulModels.IStreamChannel.IsMessageCacheTrimmingPaused"/> is <c>true</c> - growth is
    /// bounded by <see cref="MaxHistoryMessages"/> instead.
    /// </summary>
    public sealed class MessageCacheWindow
    {
        /// <summary>Keep up to 500 messages; remove 100 at a time when over the limit; stop paging in history at 2000.</summary>
        public static readonly MessageCacheWindow Recommended = new MessageCacheWindow(500, 100, 2000);

        /// <summary>Trimming starts when <see cref="StatefulModels.IStreamChannel.Messages"/> exceeds this count.</summary>
        public int MaxMessages { get; }

        /// <summary>How many messages to remove per trim. Must be less than <see cref="MaxMessages"/>.</summary>
        public int DiscardBatchSize { get; }

        /// <summary>
        /// How large <see cref="StatefulModels.IStreamChannel.Messages"/> may grow before
        /// <see cref="StatefulModels.IStreamChannel.LoadOlderMessagesAsync"/> stops paging in history. This is
        /// the total message count, not a separate budget for paged-in messages.
        /// <para>It only comes into play while
        /// <see cref="StatefulModels.IStreamChannel.IsMessageCacheTrimmingPaused"/> is <c>true</c> - which
        /// <see cref="StatefulModels.IStreamChannel.LoadOlderMessagesAsync"/> sets automatically - because
        /// otherwise <see cref="MaxMessages"/> already keeps the channel smaller than this.</para>
        /// <para>Reaching it never removes anything: paged-in history is exactly what a trim would delete.
        /// Loading simply stops until
        /// <see cref="StatefulModels.IStreamChannel.ResumeMessageCacheTrimming"/> is called. Live messages are
        /// still appended, so a channel that is never resumed can grow past this value; the SDK logs a warning
        /// once when that happens.</para>
        /// Must be greater than or equal to <see cref="MaxMessages"/>. Defaults to 4x <see cref="MaxMessages"/>.
        /// Setting it equal to <see cref="MaxMessages"/> means "never page in history beyond the normal limit".
        /// </summary>
        public int MaxHistoryMessages { get; }

        public MessageCacheWindow(int maxMessages, int discardBatchSize)
            : this(maxMessages, discardBatchSize, GetDefaultMaxHistoryMessages(maxMessages))
        {
        }

        public MessageCacheWindow(int maxMessages, int discardBatchSize, int maxHistoryMessages)
        {
            if (maxMessages <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxMessages), maxMessages,
                    $"{nameof(maxMessages)} must be greater than zero.");
            }

            if (discardBatchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(discardBatchSize), discardBatchSize,
                    $"{nameof(discardBatchSize)} must be greater than zero.");
            }

            if (discardBatchSize >= maxMessages)
            {
                throw new ArgumentOutOfRangeException(nameof(discardBatchSize), discardBatchSize,
                    $"{nameof(discardBatchSize)} must be smaller than {nameof(maxMessages)} ({maxMessages}), "
                    + "otherwise a single trim would remove every message.");
            }

            if (maxHistoryMessages < maxMessages)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHistoryMessages), maxHistoryMessages,
                    $"{nameof(maxHistoryMessages)} must be greater than or equal to {nameof(maxMessages)} "
                    + $"({maxMessages}).");
            }

            MaxMessages = maxMessages;
            DiscardBatchSize = discardBatchSize;
            MaxHistoryMessages = maxHistoryMessages;
        }

        public override string ToString()
            => $"MessageCacheWindow - MaxMessages: {MaxMessages}, DiscardBatchSize: {DiscardBatchSize}, "
               + $"MaxHistoryMessages: {MaxHistoryMessages}";

        private const int DefaultMaxHistoryMessagesMultiplier = 4;

        // Invalid values are passed through so the constructor reports the real problem instead of
        // a derived one.
        private static int GetDefaultMaxHistoryMessages(int maxMessages)
        {
            if (maxMessages <= 0)
            {
                return maxMessages;
            }

            return maxMessages > int.MaxValue / DefaultMaxHistoryMessagesMultiplier
                ? int.MaxValue
                : maxMessages * DefaultMaxHistoryMessagesMultiplier;
        }
    }
}
