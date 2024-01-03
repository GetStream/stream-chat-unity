using System;
using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.State;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Requests
{
    public class StreamMessageRequest : ISavableTo<MessageRequestInternalDTO>
    {
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

        /// <summary>
        /// Add or update custom data associated with this message. This will be accessible through <see cref="IStreamMessage.CustomData"/>
        /// </summary>
        public StreamCustomDataRequest CustomData { get; set; }

        MessageRequestInternalDTO ISavableTo<MessageRequestInternalDTO>.SaveToDto()
            => new MessageRequestInternalDTO
            {
                Attachments
                    = Attachments?.TrySaveToDtoCollection<StreamAttachmentRequest, AttachmentRequestInternalDTO>(),
                // Cid = Cid, Purposely ignored because it has no effect and endpoint already contains channel type&id
                //Html = Html, Marked in DTO as server-side only
                Id = Id,
                MentionedUsers = MentionedUsers?.ToUserIdsListOrNull(),
                ParentId = ParentId,
                PinExpires = PinExpires,
                Pinned = Pinned,
                PinnedAt = PinnedAt,
                PinnedBy = PinnedBy?.Id,
                QuotedMessageId = QuotedMessage?.Id,
                ShowInChannel = ShowInChannel,
                Silent = Silent,
                Text = Text,
                AdditionalProperties = CustomData?.ToDictionary()
            };
    }
}