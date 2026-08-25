using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;

namespace StreamChat.Core.StatefulModels
{
    /// <summary>
    /// Messages are sent by <see cref="IStreamUser"/> or <see cref="IStreamChannelMember"/> to <see cref="IStreamChannel"/>
    /// </summary>
    public interface IStreamMessage : IStreamStatefulModel
    {
        /// <summary>
        /// Event fired when a new <see cref="StreamReaction"/> was added to <see cref="IStreamMessage"/>
        /// </summary>
        event StreamMessageReactionHandler ReactionAdded;

        /// <summary>
        /// Event fired when a <see cref="StreamReaction"/> was removed from a <see cref="IStreamMessage"/>
        /// </summary>
        event StreamMessageReactionHandler ReactionRemoved;

        /// <summary>
        /// Event fired when a <see cref="StreamReaction"/> was updated on a <see cref="IStreamMessage"/>
        /// </summary>
        event StreamMessageReactionHandler ReactionUpdated;

        /// <summary>
        /// Array of message attachments
        /// </summary>
        IReadOnlyList<StreamMessageAttachment> Attachments { get; }

        /// <summary>
        /// Channel unique identifier in type:id format
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
        /// ID of the poll attached to this message
        /// </summary>
        string PollId { get; }

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
        /// Text of the message
        /// </summary>
        string Text { get; }

        /// <summary>
        /// List of users who participate in thread
        /// </summary>
        IReadOnlyList<IStreamUser> ThreadParticipants { get; }

        /// <summary>
        /// Contains type of the message
        /// </summary>
        StreamMessageType Type { get; }

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
        /// Whether this message receives realtime updates (new reactions, edits, deletions, etc.).
        /// A message is watched while its parent <see cref="IStreamChannel"/> is watched, so this
        /// always matches <see cref="IStreamChannel.IsWatched"/> of the channel it belongs to.
        ///
        /// <para>
        /// Messages from <see cref="IStreamChatClient.QueryChannelsAsync"/>,
        /// <see cref="IStreamChatClient.GetOrCreateChannelWithIdAsync"/> or a channel's
        /// <see cref="IStreamChannel.Messages"/> are watched. A message may not be watched when it
        /// comes from <see cref="IStreamChatClient.SearchMessagesAsync"/> with
        /// <see cref="Requests.StreamSearchMessagesRequest.WatchResultChannels"/> set to <c>false</c>,
        /// or from <see cref="IStreamChatClient.QueryThreadsAsync"/> with
        /// <see cref="Requests.StreamQueryThreadsRequest.Watch"/> set to <c>false</c>.
        /// </para>
        ///
        /// <para>
        /// While this is <c>false</c>, events like <see cref="ReactionAdded"/> do not fire. Call
        /// <see cref="IStreamChannel.WatchAsync"/> on the parent channel to start receiving updates
        /// for this message.
        /// </para>
        /// </summary>
        bool IsWatched { get; }

        /// <summary>
        /// Clears the message text but leaves the rest of the message data like: reactions, replies, attachments unchanged
        /// If you want to remove the message and all its components completely use the <see cref="IStreamMessage.HardDeleteAsync"/>
        /// </summary>
        Task SoftDeleteAsync();

        /// <summary>
        /// Removes the message completely along with its reactions, replies, attachments, and all other message data
        /// If you want to clear the text only use the <see cref="IStreamMessage.SoftDeleteAsync"/>
        /// </summary>
        Task HardDeleteAsync();

        /// <summary>
        /// Update message text or other parameters
        /// </summary>
        Task UpdateAsync(StreamUpdateMessageRequest streamUpdateMessageRequest); //StreamTodo: rename to UpdateOverwriteAsync

        /// <summary>
        /// Pin this message to a channel with optional expiration date
        /// </summary>
        /// <param name="expiresAt">[Optional] UTC DateTime when pin will expire</param>
        Task PinAsync(DateTime? expiresAt = null);

        /// <summary>
        /// Unpin this message from its channel
        /// </summary>
        Task UnpinAsync();

        /// <summary>
        /// <para>Translate this message's text into the given language.</para>
        ///
        /// The translation is stored on the message and available from <see cref="I18n"/> under the
        /// <c>{language}_text</c> key (e.g. "en_text" for English), alongside a <c>language</c> key
        /// naming the detected source language. Stream notifies every client watching the channel
        /// that the message was updated. Requires the auto-translation feature to be enabled for
        /// your app.
        /// </summary>
        /// <param name="language">Target language as an ISO language code, e.g. "en"</param>
        /// <remarks>https://getstream.io/chat/docs/unity/translation/?language=unity</remarks>
        Task TranslateAsync(string language);

        /// <summary>
        /// Add reaction to this message
        /// You can view reactions with:
        /// - <see cref="IStreamMessage.ReactionScores"/>,
        /// - <see cref="IStreamMessage.ReactionCounts"/>,
        /// - <see cref="IStreamMessage.ReactionScores"/>,
        /// and <see cref="IStreamMessage.ReactionCounts"/>
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
        /// <param name="type">Reaction custom key, examples: like, smile, sad, or any custom key</param>
        /// <returns></returns>
        Task DeleteReactionAsync(string type);

        /// <summary>
        /// Any user is allowed to flag a message. This triggers the message.flagged webhook event and adds the message to the inbox of your Stream Dashboard Chat Moderation webpage.
        /// </summary>
        Task FlagAsync();

        /// <summary>
        /// Mark this message as the last that was read by local user in its channel
        /// If you want to mark whole channel as read use the <see cref="IStreamChannel.MarkChannelReadAsync"/>
        ///
        /// This feature allows to track to which messages users have read in the channel
        /// </summary>
        Task MarkMessageAsLastReadAsync();

        /// <summary>
        /// Get the thread for which this message is the parent. The returned <see cref="IStreamThread"/>
        /// stays updated by realtime events.
        /// </summary>
        /// <param name="replyLimit">[Optional] Number of replies to fetch</param>
        /// <param name="participantLimit">[Optional] Number of participants to fetch</param>
        Task<IStreamThread> GetThreadAsync(int? replyLimit = null, int? participantLimit = null);

        /// <summary>
        /// Load replies of this message (a parent message of a thread). Returned messages are oldest-first
        /// and show up in <see cref="IStreamThread.LatestReplies"/> if a thread is tracked.
        ///
        /// <paramref name="idLessThan"/> and <paramref name="idGreaterThan"/> are mutually exclusive.
        /// Pass neither to load the latest <paramref name="limit"/> replies.
        /// </summary>
        /// <param name="limit">[Optional] Maximum number of replies to load. Defaults to 25</param>
        /// <param name="idLessThan">[Optional] Pagination - return replies older than this message id</param>
        /// <param name="idGreaterThan">[Optional] Pagination - return replies newer than this message id</param>
        Task<IReadOnlyList<IStreamMessage>> LoadRepliesAsync(int limit = 25, string idLessThan = null,
            string idGreaterThan = null);

        /// <summary>
        /// Mark the thread (this message must be the thread parent) as read for the local user
        /// </summary>
        Task MarkThreadAsReadAsync();

        /// <summary>
        /// Mark the thread (this message must be the thread parent) as unread for the local user
        /// </summary>
        Task MarkThreadAsUnreadAsync();

        /// <summary>
        /// Mark the channel containing this message as unread starting from this message for the local user
        /// </summary>
        Task MarkAsUnreadAsync();
    }
}