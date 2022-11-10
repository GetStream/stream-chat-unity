using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.State;
using StreamChat.Core.State.Models;
using StreamChat.Core.State.Requests;
using StreamChat.Core.State.TrackedObjects;

namespace StreamChat.Core.State.TrackedObjects
{
    /// <summary>
    /// Messages are sent by <see cref="IStreamUser"/> or <see cref="IStreamChannelMember"/> to <see cref="IStreamChannel"/>
    /// </summary>
    public interface IStreamMessage : IStreamTrackedObject
    {
        /// <summary>
        /// Array of message attachments
        /// </summary>
        IReadOnlyList<StreamMessageAttachment> Attachments { get; }

        /// <summary>
        /// Channel unique identifier in &lt;type&gt;:&lt;id&gt; format
        /// </summary>
        string Cid { get; }

        /// <summary>
        /// Contains provided slash command
        /// </summary>
        string Command { get; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        DateTimeOffset CreatedAt { get; }

        /// <summary>
        /// Date/time of deletion
        /// </summary>
        DateTimeOffset? DeletedAt { get; }

        /// <summary>
        /// Contains HTML markup of the message. Can only be set when using server-side API
        /// </summary>
        string Html { get; }

        /// <summary>
        /// Object with translations. Key `language` contains the original language key. Other keys contain translations
        /// </summary>
        IReadOnlyDictionary<string, string> I18n { get; }

        /// <summary>
        /// Message ID is unique string identifier of the message
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Contains image moderation information
        /// *** NOT IMPLEMENTED *** PLEASE SEND SUPPORT TICKET IF YOU NEED THIS FEATURE
        /// </summary>
        IReadOnlyDictionary<string, IReadOnlyList<string>> ImageLabels { get; }

        /// <summary>
        /// List of 10 latest reactions to this message
        /// </summary>
        IReadOnlyList<StreamReaction> LatestReactions { get; }

        /// <summary>
        /// List of mentioned users
        /// </summary>
        IReadOnlyList<IStreamUser> MentionedUsers { get; }

        /// <summary>
        /// Should be empty if `text` is provided. Can only be set when using server-side API
        /// </summary>
        string Mml { get; }

        /// <summary>
        /// List of 10 latest reactions of authenticated user to this message
        /// </summary>
        IReadOnlyList<StreamReaction> OwnReactions { get; }

        /// <summary>
        /// ID of parent message (thread)
        /// </summary>
        string ParentId { get; }

        /// <summary>
        /// Date when pinned message expires
        /// </summary>
        DateTimeOffset? PinExpires { get; }

        /// <summary>
        /// Whether message is pinned or not
        /// </summary>
        bool Pinned { get; }

        /// <summary>
        /// Date when message got pinned
        /// </summary>
        DateTimeOffset? PinnedAt { get; }

        /// <summary>
        /// Contains user who pinned the message
        /// </summary>
        IStreamUser PinnedBy { get; }

        /// <summary>
        /// Contains quoted message
        /// </summary>
        IStreamMessage QuotedMessage { get; }

        string QuotedMessageId { get; }

        /// <summary>
        /// An object containing number of reactions of each type. Key: reaction type (string), value: number of reactions (int)
        /// </summary>
        IReadOnlyDictionary<string, int> ReactionCounts { get; }

        /// <summary>
        /// An object containing scores of reactions of each type. Key: reaction type (string), value: total score of reactions (int)
        /// </summary>
        IReadOnlyDictionary<string, int> ReactionScores { get; }

        /// <summary>
        /// Number of replies to this message
        /// </summary>
        int? ReplyCount { get; }

        /// <summary>
        /// Whether the message was shadowed or not
        /// </summary>
        bool? Shadowed { get; }

        /// <summary>
        /// Whether thread reply should be shown in the channel as well
        /// </summary>
        bool? ShowInChannel { get; }

        /// <summary>
        /// Whether message is silent or not
        /// </summary>
        bool? Silent { get; }

        /// <summary>
        /// Text of the message. Should be empty if `mml` is provided
        /// </summary>
        string Text { get; }

        /// <summary>
        /// List of users who participate in thread
        /// </summary>
        IReadOnlyList<IStreamUser> ThreadParticipants { get; }

        /// <summary>
        /// Contains type of the message
        /// </summary>
        StreamMessageType? Type { get; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        DateTimeOffset? UpdatedAt { get; }

        /// <summary>
        /// Sender of the message. Required when using server-side API
        /// </summary>
        IStreamUser User { get; }

        bool IsDeleted { get; }

        /// <summary>
        /// Clears the message text but leaves the rest of the message unchanged e.g. reaction, replies, attachments will be untouched
        ///
        /// If you want to remove the message and all its components completely use the <see cref="IStreamMessage.HardDeleteAsync"/>
        /// </summary>
        Task SoftDeleteAsync();

        /// <summary>
        /// Removes the message completely along with its reactions, replies, attachments, etc.
        ///
        /// If you want to clear the text only use the <see cref="IStreamMessage.SoftDeleteAsync"/>
        /// </summary>
        Task HardDeleteAsync();

        /// <summary>
        /// Update message text or other parameters
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        Task UpdateAsync(StreamUpdateMessageRequest streamUpdateMessageRequest);

        /// <summary>
        /// Pin this message to a channel with optional expiration date
        /// </summary>
        /// <param name="expiresAt">[Optional] UTC DateTime when pin will expire</param>
        Task PinAsync(DateTime? expiresAt = null);

        /// <summary>
        /// Unpin this message from a channel
        /// </summary>
        Task UnpinAsync();

        /// <summary>
        /// Add reaction to this message
        /// You can view reactions via <see cref="IStreamMessage.ReactionScores"/>, <see cref="IStreamMessage.ReactionCounts"/>, <see cref="IStreamMessage.ReactionScores"/> and <see cref="StreamMessage.ReactionCounts"/>
        /// </summary>
        /// <param name="type">Reaction custom key, examples: like, smile, sad, etc. or any custom string</param>
        /// <param name="score">[Optional] Reaction score, by default it counts as 1</param>
        /// <param name="enforceUnique">[Optional] Whether to replace all existing user reactions</param>
        /// <param name="skipMobilePushNotifications">[Optional] Skips any mobile push notifications</param>
        Task SendReactionAsync(string type, int score = 1, bool enforceUnique = false,
            bool skipMobilePushNotifications = false);

        /// <summary>
        /// Delete reaction
        /// </summary>
        /// <param name="type">Reaction custom key, examples: like, smile, sad, etc. You can use any custom key</param>
        /// <returns></returns>
        Task DeleteReactionAsync(string type);

        /// <summary>
        /// Any user is allowed to flag a message. This triggers the message.flagged webhook event and adds the message to the inbox of your Stream Dashboard Chat Moderation view.
        /// </summary>
        Task FlagAsync();

        /// <summary>
        /// Mark this message as the last that was read by local user in this channel
        /// If you want to mark whole channel as read use the <see cref="IStreamChannel.MarkChannelReadAsync"/>
        ///
        /// This feature allows to track to which message users have read the channel
        /// </summary>
        Task MarkMessageAsLastReadAsync();
    }
}