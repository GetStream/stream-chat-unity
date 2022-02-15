using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    /// <summary>
    /// Represents any chat message
    /// </summary>
    public class MessageRequest : RequestObjectBase, ISavableTo<MessageRequestDTO>
    {
        /// <summary>
        /// Array of message attachments
        /// </summary>
        public ICollection<AttachmentRequest> Attachments { get; set; } =
            new Collection<AttachmentRequest>();

        /// <summary>
        /// Channel unique identifier in <type>:<id> format
        /// </summary>
        public ICollection<double> Cid { get; set; }

        /// <summary>
        /// Contains HTML markup of the message. Can only be set when using server-side API
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// Message ID is unique string identifier of the message
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// List of mentioned users
        /// </summary>
        public ICollection<string> MentionedUsers { get; set; }

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
        public ICollection<double> PinnedBy { get; set; }

        public string QuotedMessageId { get; set; }

        /// <summary>
        /// An object containing scores of reactions of each type. Key: reaction type (string), value: total score of reactions (int)
        /// </summary>
        public ICollection<double> ReactionScores { get; set; }

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
        /// Sender of the message. Required when using server-side API
        /// </summary>
        public UserObjectRequest User { get; set; }

        public string UserId { get; set; }

        MessageRequestDTO ISavableTo<MessageRequestDTO>.SaveToDto() =>
            new MessageRequestDTO
            {
                Attachments = Attachments?.TrySaveToDtoCollection<AttachmentRequest, AttachmentRequestDTO>(),
                Cid = Cid,
                Html = Html,
                Id = Id,
                MentionedUsers = MentionedUsers,
                Mml = Mml,
                ParentId = ParentId,
                PinExpires = PinExpires,
                Pinned = Pinned,
                PinnedAt = PinnedAt,
                PinnedBy = PinnedBy,
                QuotedMessageId = QuotedMessageId,
                ReactionScores = ReactionScores,
                ShowInChannel = ShowInChannel,
                Silent = Silent,
                Text = Text,
                User = User.TrySaveToDto(),
                UserId = UserId,
                AdditionalProperties = AdditionalProperties,
            };
    }
}