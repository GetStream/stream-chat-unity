using System;
using System.Collections.Generic;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public class Message : ModelBase, ILoadableFrom<MessageDTO, Message>
    {
        /// <summary>
        /// Array of message attachments
        /// </summary>
        public IList<Attachment> Attachments { get; set; }

        /// <summary>
        /// Whether `before_message_send webhook` failed or not. Field is only accessible in push webhook
        /// </summary>
        public bool? BeforeMessageSendFailed { get; set; }

        /// <summary>
        /// Channel unique identifier in &lt;type&gt;:&lt;id&gt; format
        /// </summary>
        public string Cid { get; set; }

        /// <summary>
        /// Contains provided slash command
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Date/time of deletion
        /// </summary>
        public DateTimeOffset? DeletedAt { get; set; }

        /// <summary>
        /// Contains HTML markup of the message. Can only be set when using server-side API
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// Object with translations. Key `language` contains the original language key. Other keys contain translations
        /// </summary>
        public IDictionary<string, string> I18n { get; set; }

        /// <summary>
        /// Message ID is unique string identifier of the message
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Contains image moderation information
        /// </summary>
        public IDictionary<string, ICollection<string>> ImageLabels { get; set; }

        /// <summary>
        /// List of 10 latest reactions to this message
        /// </summary>
        public IList<Reaction> LatestReactions { get; set; }

        /// <summary>
        /// List of mentioned users
        /// </summary>
        public IList<User> MentionedUsers { get; set; }

        /// <summary>
        /// Should be empty if `text` is provided. Can only be set when using server-side API
        /// </summary>
        public string Mml { get; set; }

        /// <summary>
        /// List of 10 latest reactions of authenticated user to this message
        /// </summary>
        public IList<Reaction> OwnReactions { get; set; }

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
        public User PinnedBy { get; set; }

        /// <summary>
        /// Contains quoted message
        /// </summary>
        public Message QuotedMessage { get; set; }

        public string QuotedMessageId { get; set; }

        /// <summary>
        /// An object containing number of reactions of each type. Key: reaction type (string), value: number of reactions (int)
        /// </summary>
        public IDictionary<string, double> ReactionCounts { get; set; }

        /// <summary>
        /// An object containing scores of reactions of each type. Key: reaction type (string), value: total score of reactions (int)
        /// </summary>
        public IDictionary<string, double> ReactionScores { get; set; }

        /// <summary>
        /// Number of replies to this message
        /// </summary>
        public int? ReplyCount { get; set; }

        /// <summary>
        /// Whether the message was shadowed or not
        /// </summary>
        public bool? Shadowed { get; set; }

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
        /// List of users who participate in thread
        /// </summary>
        public ICollection<User> ThreadParticipants { get; set; }

        /// <summary>
        /// Contains type of the message
        /// </summary>
        public MessageType? Type { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Sender of the message. Required when using server-side API
        /// </summary>
        public User User { get; set; }

        Message ILoadableFrom<MessageDTO, Message>.LoadFromDto(MessageDTO dto)
        {
            AdditionalProperties = dto.AdditionalProperties;
            Attachments = Attachments.TryLoadFromDtoCollection(dto.Attachments);
            BeforeMessageSendFailed = dto.BeforeMessageSendFailed;
            Cid = dto.Cid;
            Command = dto.Command;
            CreatedAt = dto.CreatedAt;
            DeletedAt = dto.DeletedAt;
            Html = dto.Html;
            I18n = dto.I18n;
            Id = dto.Id;
            ImageLabels = dto.ImageLabels;
            LatestReactions = LatestReactions.TryLoadFromDtoCollection(dto.LatestReactions);
            MentionedUsers = MentionedUsers.TryLoadFromDtoCollection(dto.MentionedUsers);
            Mml = dto.Mml;
            OwnReactions = OwnReactions.TryLoadFromDtoCollection(dto.OwnReactions);
            ParentId = dto.ParentId;
            PinExpires = dto.PinExpires;
            Pinned = dto.Pinned;
            PinnedAt = dto.PinnedAt;
            PinnedBy = PinnedBy.TryLoadFromDto<UserObjectDTO, User>(dto.PinnedBy);
            QuotedMessage = QuotedMessage.TryLoadFromDto(dto.QuotedMessage);
            QuotedMessageId = dto.QuotedMessageId;
            ReactionCounts = dto.ReactionCounts;
            ReactionScores = dto.ReactionScores;
            ReplyCount = dto.ReplyCount;
            Shadowed = dto.Shadowed;
            ShowInChannel = dto.ShowInChannel;
            Silent = dto.Silent;
            Text = dto.Text;
            ThreadParticipants = ThreadParticipants.TryLoadFromDtoCollection(dto.ThreadParticipants);
            Type = dto.Type;
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);

            return this;
        }
    }
}