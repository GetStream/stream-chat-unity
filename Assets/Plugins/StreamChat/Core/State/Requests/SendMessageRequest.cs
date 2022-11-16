using System;
using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.State.Requests
{
    public class StreamSendMessageRequest : ISavableTo<SendMessageRequestInternalInternalDTO>
    {
        /// <summary>
        /// Make the message a pending message. This message will not be viewable to others until it is committed.
        /// </summary>
        public bool? IsPendingMessage { get; set; }

        public Dictionary<string, string> PendingMessageMetadata { get; set; }

        /// <summary>
        /// Do not try to enrich the links within message
        /// </summary>
        public bool? SkipEnrichUrl { get; set; }

        /// <summary>
        /// Disables all push notifications for this message
        /// </summary>
        public bool? SkipPush { get; set; }

        #region MessageRequest

        /// <summary>
        /// Array of message attachments
        /// </summary>
        public List<StreamAttachmentRequest> Attachments { get; set; }

        /// <summary>
        /// Message ID is unique string identifier of the message
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// List of mentioned users
        /// </summary>
        public List<IStreamUser> MentionedUsers { get; set; }

        /// <summary>
        /// Should be empty if `text` is provided. Can only be set when using server-side API
        /// </summary>
        public string Mml { get; set; }

        /// <summary>
        /// ID of parent message (thread)
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Date when pinned message expires
        /// </summary>
        public DateTimeOffset? PinExpires { get; set; }

        /// <summary>
        /// Whether message is pinned or not
        /// </summary>
        public bool? Pinned { get; set; }

        /// <summary>
        /// Date when message got pinned
        /// </summary>
        public DateTimeOffset? PinnedAt { get; set; }

        /// <summary>
        /// Contains user who pinned the message
        /// </summary>
        public IStreamUser PinnedBy { get; set; }

        public IStreamMessage QuotedMessage { get; set; }

        /// <summary>
        /// Whether thread reply should be shown in the channel as well
        /// </summary>
        public bool? ShowInChannel { get; set; }

        /// <summary>
        /// Whether message is silent or not
        /// </summary>
        public bool? Silent { get; set; }

        /// <summary>
        /// Text of the message. Should be empty if `mml` is provided
        /// </summary>
        public string Text { get; set; }

        #endregion

        SendMessageRequestInternalInternalDTO ISavableTo<SendMessageRequestInternalInternalDTO>.SaveToDto()
        {
            var messageRequestDto = new MessageRequestInternalInternalDTO()
            {
                Attachments = Attachments?.TrySaveToDtoCollection<StreamAttachmentRequest, AttachmentRequestInternalDTO>(),
                // Cid = Cid, Purposely ignored because it has no effect and endpoint already contains channel type&id
                //Html = Html, Marked in DTO as server-side only
                Id = Id,
                MentionedUsers = MentionedUsers?.ToUserIdsListOrNull(),
                Mml = Mml,
                ParentId = ParentId,
                PinExpires = PinExpires,
                Pinned = Pinned,
                PinnedAt = PinnedAt,
                PinnedBy = PinnedBy?.Id,
                QuotedMessageId = QuotedMessage?.Id,
                ShowInChannel = ShowInChannel,
                Silent = Silent,
                Text = Text,
            };

            return new SendMessageRequestInternalInternalDTO
            {
                IsPendingMessage = IsPendingMessage,
                Message = messageRequestDto,
                PendingMessageMetadata = PendingMessageMetadata,
                SkipEnrichUrl = SkipEnrichUrl,
                SkipPush = SkipPush,
            };
        }
    }
}