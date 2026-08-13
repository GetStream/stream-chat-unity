using System;

namespace StreamChat.Core.Configs
{
    /// <summary>
    /// Limits how many messages a channel keeps in the local cache.
    /// Assign to <see cref="IStreamClientConfig.DefaultMessageCacheWindow"/> or
    /// <see cref="StatefulModels.IStreamChannel.OverrideMessageCacheWindow"/>.
    /// Trimming runs in batches of <see cref="DiscardBatchSize"/> once the count exceeds
    /// <see cref="MaxMessages"/>.
    /// </summary>
    public sealed class MessageCacheWindow
    {
        /// <summary>Keep up to 500 messages; remove 100 at a time when over the limit.</summary>
        public static readonly MessageCacheWindow Recommended = new MessageCacheWindow(500, 100);

        /// <summary>Trimming starts when <see cref="StatefulModels.IStreamChannel.Messages"/> exceeds this count.</summary>
        public int MaxMessages { get; }

        /// <summary>How many messages to remove per trim. Must be less than <see cref="MaxMessages"/>.</summary>
        public int DiscardBatchSize { get; }

        public MessageCacheWindow(int maxMessages, int discardBatchSize)
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

            MaxMessages = maxMessages;
            DiscardBatchSize = discardBatchSize;
        }

        public override string ToString()
            => $"MessageCacheWindow - MaxMessages: {MaxMessages}, DiscardBatchSize: {DiscardBatchSize}";
    }
}
