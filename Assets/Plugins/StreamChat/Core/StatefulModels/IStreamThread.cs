using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.Models;

namespace StreamChat.Core.StatefulModels
{
    /// <summary>
    /// Generic thread event handler
    /// </summary>
    public delegate void StreamThreadChangeHandler(IStreamThread thread);

    /// <summary>
    /// Thread reply event handler
    /// </summary>
    public delegate void StreamThreadReplyHandler(IStreamThread thread, IStreamMessage reply);

    /// <summary>
    /// Thread read state event handler
    /// </summary>
    public delegate void StreamThreadReadHandler(IStreamThread thread);

    /// <summary>
    /// Stateful thread model. A thread groups replies to a parent <see cref="IStreamMessage"/> in a <see cref="IStreamChannel"/>.
    /// You can obtain instances using <see cref="IStreamChatClient.QueryThreadsAsync"/>, <see cref="IStreamChatClient.GetThreadAsync"/>
    /// or by calling <see cref="IStreamMessage.GetThreadAsync"/>.
    /// </summary>
    public interface IStreamThread : IStreamStatefulModel
    {
        /// <summary>
        /// Event fired when this thread was updated (e.g. its title or custom data changed)
        /// </summary>
        event StreamThreadChangeHandler Updated;

        /// <summary>
        /// Event fired when a new reply was received in this thread
        /// </summary>
        event StreamThreadReplyHandler ReplyReceived;

        /// <summary>
        /// Event fired when the read state of this thread changed
        /// </summary>
        event StreamThreadReadHandler ReadStateChanged;

        /// <summary>
        /// Number of currently active participants in this thread
        /// </summary>
        int? ActiveParticipantCount { get; }

        /// <summary>
        /// The channel this thread belongs to
        /// </summary>
        IStreamChannel Channel { get; }

        /// <summary>
        /// Channel CID of the channel this thread belongs to
        /// </summary>
        string ChannelCid { get; }

        /// <summary>
        /// Date/time the thread was created
        /// </summary>
        DateTimeOffset CreatedAt { get; }

        /// <summary>
        /// User who created the thread
        /// </summary>
        IStreamUser CreatedBy { get; }

        /// <summary>
        /// Id of the user who created the thread
        /// </summary>
        string CreatedByUserId { get; }

        /// <summary>
        /// Date/time of when this thread was deleted
        /// </summary>
        DateTimeOffset? DeletedAt { get; }

        /// <summary>
        /// Date/time of the last reply in the thread
        /// </summary>
        DateTimeOffset? LastMessageAt { get; }

        /// <summary>
        /// Latest replies in the thread (oldest-first)
        /// </summary>
        IReadOnlyList<IStreamMessage> LatestReplies { get; }

        /// <summary>
        /// Parent <see cref="IStreamMessage"/>
        /// </summary>
        IStreamMessage ParentMessage { get; }

        /// <summary>
        /// Parent message id (also the unique identifier of this thread)
        /// </summary>
        string ParentMessageId { get; }

        /// <summary>
        /// Total number of participants in this thread
        /// </summary>
        int? ParticipantCount { get; }

        /// <summary>
        /// Read state of users participating in this thread
        /// </summary>
        IReadOnlyList<StreamRead> Read { get; }

        /// <summary>
        /// Total number of replies in this thread
        /// </summary>
        int? ReplyCount { get; }

        /// <summary>
        /// All thread participants (including those who left)
        /// </summary>
        IReadOnlyList<StreamThreadParticipant> ThreadParticipants { get; }

        /// <summary>
        /// Optional title of the thread
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        DateTimeOffset UpdatedAt { get; }

        /// <summary>
        /// Refresh this thread from the server. Updates <see cref="LatestReplies"/>, <see cref="ThreadParticipants"/> and other state.
        /// </summary>
        /// <param name="replyLimit">[Optional] Number of replies to fetch</param>
        /// <param name="participantLimit">[Optional] Number of participants to fetch</param>
        Task RefreshAsync(int? replyLimit = null, int? participantLimit = null);

        /// <summary>
        /// Load older replies (paginates before the oldest currently loaded reply)
        /// </summary>
        /// <param name="limit">[Optional] Maximum number of replies to load. Defaults to 25</param>
        /// <returns>The newly loaded replies (oldest-first)</returns>
        Task<IReadOnlyList<IStreamMessage>> LoadOlderRepliesAsync(int limit = 25);

        /// <summary>
        /// Update this thread in a partial mode. You can selectively set and unset fields (e.g. <c>title</c>, custom data).
        /// </summary>
        /// <param name="setFields">[Optional] Fields to set with new values</param>
        /// <param name="unsetFields">[Optional] Fields to unset (remove)</param>
        Task UpdatePartialAsync(IDictionary<string, object> setFields = null,
            IEnumerable<string> unsetFields = null);

        /// <summary>
        /// Mark this thread as read for the local user
        /// </summary>
        Task MarkReadAsync();

        /// <summary>
        /// Mark this thread as unread starting from the parent message for the local user
        /// </summary>
        Task MarkUnreadAsync();
    }
}
