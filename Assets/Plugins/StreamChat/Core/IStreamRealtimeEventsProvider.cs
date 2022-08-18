using System;
using StreamChat.Core.Events;

namespace StreamChat.Core
{
    /// <summary>
    /// Triggers realtime events received from server
    /// </summary>
    public interface IStreamRealtimeEventsProvider
    {
        /// <summary>
        /// Debug log for received event
        /// </summary>
        event Action<string> EventReceived;

        /// <summary>
        /// Event raised when a new message has been sent to a watched channel.
        ///
        /// Use <see cref="EventMessageNew.Cid"/> to know which channel it belongs to.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventMessageNew> MessageReceived;

        /// <summary>
        /// Event raised when a message has been updated on a watched channel.
        ///
        /// Use <see cref="EventMessageUpdated.Cid"/> to know which channel it belongs to.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventMessageUpdated> MessageUpdated;

        /// <summary>
        /// Event raised when a message has been deleted from a watched channel.
        ///
        /// Use <see cref="EventMessageDeleted.Cid"/> to know which channel it belongs to.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventMessageDeleted> MessageDeleted;

        /// <summary>
        /// Event raised when a when a watched channel is marked as read.
        ///
        /// Use <see cref="EventMessageRead.Cid"/> to know which channel it belongs to.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventMessageRead> MessageRead;

        /// <summary>
        /// Event raised when a reaction has been added to the message of a watched channel.
        ///
        /// Use <see cref="EventReactionNew.Cid"/> & <see cref="EventReactionNew.Message"/> to know which channel & message it belongs to.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventReactionNew> ReactionReceived;

        /// <summary>
        /// Event raised when a reaction has been updated in the message of a watched channel.
        ///
        /// Use <see cref="EventReactionNew.Cid"/> & <see cref="EventReactionNew.Message"/> to know which channel & message it belongs to.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventReactionUpdated> ReactionUpdated;

        /// <summary>
        /// Event raised when a reaction has been deleted from the message of a watched channel.
        ///
        /// Use <see cref="EventReactionNew.Cid"/> & <see cref="EventReactionNew.Message"/> to know which channel & message it belongs to.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventReactionDeleted> ReactionDeleted;

        /// <summary>
        /// Notification Event raised when the total count of unread messages (across all channels the user is a member) changes.
        ///
        /// Use <see cref="EventNotificationMarkRead.Cid"/> & <see cref="EventNotificationMarkRead.Channel"/> to know which channel it belongs to.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationMarkRead> NotificationMarkRead;

        /// <summary>
        /// Notification Event raised when a new message is sent to channel the user is member of.
        ///
        /// Notifications are sent to all channel members regardless of whether they're actively watching this channel.
        /// Use <see cref="EventNotificationMessageNew.Cid"/> & <see cref="EventNotificationMessageNew.Message"/> to know which channel & message it belongs to.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationMessageNew> NotificationMessageReceived;

        event Action<EventTypingStart> TypingStarted;
        event Action<EventTypingStop> TypingStopped;
    }
}