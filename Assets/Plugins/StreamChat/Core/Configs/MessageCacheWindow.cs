using System;

namespace StreamChat.Core.Configs
{
    /// <summary>
    /// Limits how many messages a channel keeps in the local cache.
    /// Assign to <see cref="IStreamClientConfig.DefaultMessageCacheWindow"/> or
    /// <see cref="StatefulModels.IStreamChannel.OverrideMessageCacheWindow"/>.
    /// Trimming runs in batches of <see cref="DiscardBatchSize"/> once the count exceeds
    /// <see cref="MaxMessages"/>, or once it exceeds <see cref="AbsoluteMaxMessages"/> while
    /// trimming is paused.
    /// </summary>
    public sealed class MessageCacheWindow
    {
        /// <summary>Keep up to 500 messages (2000 while paused); remove 100 at a time when over the limit.</summary>
        public static readonly MessageCacheWindow Recommended = new MessageCacheWindow(500, 100, 2000);

        /// <summary>Trimming starts when <see cref="StatefulModels.IStreamChannel.Messages"/> exceeds this count.</summary>
        public int MaxMessages { get; }

        /// <summary>How many messages to remove per trim. Must be less than <see cref="MaxMessages"/>.</summary>
        public int DiscardBatchSize { get; }

        /// <summary>
        /// Upper bound that applies while <see cref="StatefulModels.IStreamChannel.IsMessageCacheTrimmingPaused"/>
        /// is <c>true</c> - for example while the user reads history loaded by
        /// <see cref="StatefulModels.IStreamChannel.LoadOlderMessagesAsync"/>. Pausing widens the window to this
        /// value instead of disabling it, so a channel stays bounded even if
        /// <see cref="StatefulModels.IStreamChannel.ResumeMessageCacheTrimming"/> is never called.
        /// Must be greater than or equal to <see cref="MaxMessages"/>. Defaults to 4x <see cref="MaxMessages"/>.
        /// </summary>
        public int AbsoluteMaxMessages { get; }

        public MessageCacheWindow(int maxMessages, int discardBatchSize)
            : this(maxMessages, discardBatchSize, GetDefaultAbsoluteMaxMessages(maxMessages))
        {
        }

        public MessageCacheWindow(int maxMessages, int discardBatchSize, int absoluteMaxMessages)
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

            if (absoluteMaxMessages < maxMessages)
            {
                throw new ArgumentOutOfRangeException(nameof(absoluteMaxMessages), absoluteMaxMessages,
                    $"{nameof(absoluteMaxMessages)} must be greater than or equal to {nameof(maxMessages)} "
                    + $"({maxMessages}).");
            }

            MaxMessages = maxMessages;
            DiscardBatchSize = discardBatchSize;
            AbsoluteMaxMessages = absoluteMaxMessages;
        }

        public override string ToString()
            => $"MessageCacheWindow - MaxMessages: {MaxMessages}, DiscardBatchSize: {DiscardBatchSize}, "
               + $"AbsoluteMaxMessages: {AbsoluteMaxMessages}";

        private const int DefaultAbsoluteMaxMessagesMultiplier = 4;

        // Invalid values are passed through so the constructor reports the real problem instead of
        // a derived one.
        private static int GetDefaultAbsoluteMaxMessages(int maxMessages)
        {
            if (maxMessages <= 0)
            {
                return maxMessages;
            }

            return maxMessages > int.MaxValue / DefaultAbsoluteMaxMessagesMultiplier
                ? int.MaxValue
                : maxMessages * DefaultAbsoluteMaxMessagesMultiplier;
        }
    }
}
