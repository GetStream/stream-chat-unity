using System;
using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.State;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Requests
{
    /// <summary>
    /// Partial update for <see cref="IStreamMessage.UpdatePartialAsync"/>.
    /// Only properties you set are sent; everything else on the message is kept.
    /// This is the opposite of <see cref="StreamUpdateMessageRequest"/> used by
    /// <see cref="IStreamMessage.UpdateOverwriteAsync"/>, where omitted fields (including
    /// <see cref="CustomData"/>) are deleted.
    /// Custom data is flattened into the PUT <c>set</c> bag as top-level keys, matching the Chat API.
    /// Nested paths like "details.status" are allowed as custom keys.
    /// </summary>
    public sealed class StreamUpdateMessagePartialRequest
    {
        /// <summary>
        /// Message attachments. Null = leave unchanged. An empty list replaces attachments with none.
        /// </summary>
        public List<StreamAttachmentRequest> Attachments { get; set; }

        /// <summary>
        /// Mentioned users. Null = leave unchanged. An empty list clears mentions.
        /// Sent as user id strings.
        /// </summary>
        public List<IStreamUser> MentionedUsers { get; set; }

        /// <summary>
        /// Date when pinned message expires. Null = leave unchanged.
        /// </summary>
        public DateTimeOffset? PinExpires { get; set; }

        /// <summary>
        /// Whether the message is pinned. Null = leave unchanged.
        /// Prefer <see cref="IStreamMessage.PinAsync"/> / <see cref="IStreamMessage.UnpinAsync"/>
        /// when you also need <see cref="IStreamChannel.PinnedMessages"/> kept in sync.
        /// </summary>
        public bool? Pinned { get; set; }

        /// <summary>
        /// Date when the message was pinned. Null = leave unchanged.
        /// </summary>
        public DateTimeOffset? PinnedAt { get; set; }

        /// <summary>
        /// Quoted message id. Null = leave unchanged.
        /// </summary>
        public string QuotedMessageId { get; set; }

        /// <summary>
        /// Whether a thread reply should also appear in the channel. Null = leave unchanged.
        /// Changing this updates the message instance only; it does not add/remove it from
        /// <see cref="IStreamChannel.Messages"/>.
        /// </summary>
        public bool? ShowInChannel { get; set; }

        /// <summary>
        /// Whether the message is silent. Null = leave unchanged.
        /// </summary>
        public bool? Silent { get; set; }

        /// <summary>
        /// Message text. Null = leave unchanged.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Custom keys to set. Flattened into <c>set</c> next to reserved fields (not a nested object).
        /// Null or empty = do not change existing custom data. Unlike overwrite, this does not wipe keys
        /// you omit.
        /// </summary>
        public StreamCustomDataRequest CustomData { get; set; }

        /// <summary>
        /// Field names to remove (reserved or custom). Empty is ignored when something is being set.
        /// </summary>
        public IEnumerable<string> Unset { get; set; }

        /// <summary>
        /// If true, do not scrape URLs in the text for link attachments.
        /// </summary>
        public bool? SkipEnrichUrl { get; set; }

        internal Dictionary<string, object> ToSetDictionary()
        {
            var set = new Dictionary<string, object>();

            if (CustomData != null)
            {
                foreach (var kvp in CustomData.ToDictionary())
                {
                    set[kvp.Key] = kvp.Value;
                }
            }

            if (Attachments != null)
            {
                set["attachments"] = Attachments
                    .TrySaveToDtoCollection<StreamAttachmentRequest, AttachmentRequestInternalDTO>();
            }

            if (MentionedUsers != null)
            {
                set["mentioned_users"] = MentionedUsers.ToUserIdsListOrNull();
            }

            if (PinExpires.HasValue)
            {
                set["pin_expires"] = PinExpires.Value;
            }

            if (Pinned.HasValue)
            {
                set["pinned"] = Pinned.Value;
            }

            if (PinnedAt.HasValue)
            {
                set["pinned_at"] = PinnedAt.Value;
            }

            if (QuotedMessageId != null)
            {
                set["quoted_message_id"] = QuotedMessageId;
            }

            if (ShowInChannel.HasValue)
            {
                set["show_in_channel"] = ShowInChannel.Value;
            }

            if (Silent.HasValue)
            {
                set["silent"] = Silent.Value;
            }

            if (Text != null)
            {
                set["text"] = Text;
            }

            return set;
        }
    }
}
