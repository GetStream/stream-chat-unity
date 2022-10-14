using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State.Models;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.State.TrackedObjects
{
    /// <summary>
    /// Message belonging to a <see cref="StreamChannel"/>
    /// </summary>
    public class StreamMessage : StreamTrackedObjectBase<StreamMessage>, IUpdateableFrom<MessageInternalDTO, StreamMessage>
    {
        /// <summary>
        /// Array of message attachments
        /// </summary>
        public IReadOnlyList<StreamMessageAttachment> Attachments => _attachments;

        /// <summary>
        /// Whether `before_message_send webhook` failed or not. Field is only accessible in push webhook
        /// </summary>
        public bool? BeforeMessageSendFailed { get; private set; }

        /// <summary>
        /// Channel unique identifier in &lt;type&gt;:&lt;id&gt; format
        /// </summary>
        public string Cid { get; private set; }

        /// <summary>
        /// Contains provided slash command
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; private set; }

        /// <summary>
        /// Date/time of deletion
        /// </summary>
        public System.DateTimeOffset? DeletedAt { get; private set; }

        /// <summary>
        /// Contains HTML markup of the message. Can only be set when using server-side API
        /// </summary>
        public string Html { get; private set; }

        /// <summary>
        /// Object with translations. Key `language` contains the original language key. Other keys contain translations
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> I18n { get; private set; }

        /// <summary>
        /// Message ID is unique string identifier of the message
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Contains image moderation information
        /// </summary>
        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> ImageLabels { get; private set; }

        /// <summary>
        /// List of 10 latest reactions to this message
        /// </summary>
        public IReadOnlyList<StreamReaction> LatestReactions => _latestReactions;

        /// <summary>
        /// List of mentioned users
        /// </summary>
        public IReadOnlyList<StreamUser> MentionedUsers => _mentionedUsers;

        /// <summary>
        /// Should be empty if `text` is provided. Can only be set when using server-side API
        /// </summary>
        public string Mml { get; private set; }

        /// <summary>
        /// List of 10 latest reactions of authenticated user to this message
        /// </summary>
        public IReadOnlyList<StreamReaction> OwnReactions => _ownReactions;

        /// <summary>
        /// ID of parent message (thread)
        /// </summary>
        public string ParentId { get; private set; }

        /// <summary>
        /// Date when pinned message expires
        /// </summary>
        public System.DateTimeOffset? PinExpires { get; private set; }

        /// <summary>
        /// Whether message is pinned or not
        /// </summary>
        public bool? Pinned { get; private set; }

        /// <summary>
        /// Date when message got pinned
        /// </summary>
        public System.DateTimeOffset? PinnedAt { get; private set; }

        /// <summary>
        /// Contains user who pinned the message
        /// </summary>
        public StreamUser PinnedBy { get; private set; }

        /// <summary>
        /// Contains quoted message
        /// </summary>
        public StreamMessage QuotedMessage { get; private set; }

        public string QuotedMessageId { get; private set; }

        /// <summary>
        /// An object containing number of reactions of each type. Key: reaction type (string), value: number of reactions (int)
        /// </summary>
        public IReadOnlyDictionary<string, int> ReactionCounts => _reactionCounts;

        /// <summary>
        /// An object containing scores of reactions of each type. Key: reaction type (string), value: total score of reactions (int)
        /// </summary>
        public IReadOnlyDictionary<string, int> ReactionScores => _reactionScores;

        /// <summary>
        /// Number of replies to this message
        /// </summary>
        public int? ReplyCount { get; private set; }

        /// <summary>
        /// Whether the message was shadowed or not
        /// </summary>
        public bool? Shadowed { get; private set; }

        /// <summary>
        /// Whether thread reply should be shown in the channel as well
        /// </summary>
        public bool? ShowInChannel { get; private set; }

        /// <summary>
        /// Whether message is silent or not
        /// </summary>
        public bool? Silent { get; private set; }

        /// <summary>
        /// Text of the message. Should be empty if `mml` is provided
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// List of users who participate in thread
        /// </summary>
        public IReadOnlyList<StreamUser> ThreadParticipants => _threadParticipants;

        /// <summary>
        /// Contains type of the message
        /// </summary>
        public StreamMessageType? Type { get; private set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; private set; }

        /// <summary>
        /// Sender of the message. Required when using server-side API
        /// </summary>
        public StreamUser User { get; private set; }

        /// <summary>
        /// Clears the message text but leaves the rest of the message unchanged e.g. reaction, replies, attachments will be untouched
        ///
        /// If you want to remove the message and all its components completely use the <see cref="HardDelete"/>
        /// </summary>
        public void SoftDelete()
        {
            //Todo: implement
        }

        /// <summary>
        /// Removes the message completely along with its reactions, replies, attachments, etc.
        ///
        /// If you want to clear the text only use the <see cref="SoftDelete"/>
        /// </summary>
        public void HardDelete()
        {
            // Todo: implement
        }

        void IUpdateableFrom<MessageInternalDTO, StreamMessage>.UpdateFromDto(MessageInternalDTO dto, ICache cache)
        {
            _attachments.TryReplaceRegularObjectsFromDto(dto.Attachments, cache);
            BeforeMessageSendFailed = dto.BeforeMessageSendFailed;
            Cid = dto.Cid;
            Command = dto.Command;
            CreatedAt = dto.CreatedAt;
            DeletedAt = dto.DeletedAt;
            Html = dto.Html;
            I18n = dto.I18n;
            Id = dto.Id;
            ImageLabels = dto.ImageLabels;
            _latestReactions.TryReplaceRegularObjectsFromDto(dto.LatestReactions, cache);
            _mentionedUsers.TryReplaceTrackedObjects(dto.MentionedUsers, cache.Users);
            Mml = dto.Mml;
            _ownReactions.TryReplaceRegularObjectsFromDto(dto.OwnReactions, cache);
            ParentId = dto.ParentId;
            PinExpires = dto.PinExpires;
            Pinned = dto.Pinned;
            PinnedAt = dto.PinnedAt;
            PinnedBy = cache.TryCreateOrUpdate(dto.PinnedBy);
            QuotedMessage = cache.TryCreateOrUpdate(dto.QuotedMessage);
            QuotedMessageId = dto.QuotedMessageId;
            _reactionCounts.TryReplaceValuesFromDto(dto.ReactionCounts);
            _reactionScores.TryReplaceValuesFromDto(dto.ReactionScores);
            ReplyCount = dto.ReplyCount;
            Shadowed = dto.Shadowed;
            ShowInChannel = dto.ShowInChannel;
            Silent = dto.Silent;
            Text = dto.Text;
            _mentionedUsers.TryReplaceTrackedObjects(dto.MentionedUsers, cache.Users);
            _threadParticipants.TryReplaceTrackedObjects(dto.ThreadParticipants, cache.Users);
            Type = dto.Type.TryConvertToStreamMessageType();
            UpdatedAt = dto.UpdatedAt;
            User = cache.TryCreateOrUpdate(dto.User);
        }

        internal StreamMessage(string uniqueId, IRepository<StreamMessage> repository, ITrackedObjectContext context)
            : base(uniqueId, repository, context)
        {
        }

        protected override StreamMessage Self => this;

        protected override string InternalUniqueId
        {
            get => Id;
            set => Id = value;
        }

        // StreamTodo: change to lazy loading
        private readonly List<StreamMessageAttachment> _attachments = new List<StreamMessageAttachment>();
        private readonly List<StreamReaction> _latestReactions = new List<StreamReaction>();
        private readonly List<StreamUser> _mentionedUsers = new List<StreamUser>();
        private readonly List<StreamReaction> _ownReactions = new List<StreamReaction>();
        private readonly Dictionary<string, int> _reactionCounts = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _reactionScores = new Dictionary<string, int>();
        private readonly List<StreamUser> _threadParticipants = new List<StreamUser>();
    }
}