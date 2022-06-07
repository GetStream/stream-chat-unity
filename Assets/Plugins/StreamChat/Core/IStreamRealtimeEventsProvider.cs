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
        /// Raw serialized event received from server. Can be useful for debugging purposes.
        /// For production please used events with typed data structures
        /// </summary>
        event Action<string> EventReceived;

        /// <summary>
        /// Event raised when a new message has been received to a watched channel.
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
    }
}