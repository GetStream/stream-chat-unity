using System;
using StreamChat.Core.LowLevelClient.Events;

namespace StreamChat.Core.LowLevelClient
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
        /// Use <see cref="EventMessageNew.Cid"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventMessageNew> MessageReceived;

        /// <summary>
        /// Event raised when a message has been updated on a watched channel.
        ///
        /// Use <see cref="EventMessageUpdated.Cid"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventMessageUpdated> MessageUpdated;

        /// <summary>
        /// Event raised when a message has been deleted from a watched channel.
        ///
        /// Use <see cref="EventMessageDeleted.Cid"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventMessageDeleted> MessageDeleted;

        /// <summary>
        /// Event raised when a watched channel is marked as read.
        ///
        /// Use <see cref="EventMessageRead.Cid"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventMessageRead> MessageRead;

        /// <summary>
        /// Event raised when a channel is updated
        ///
        /// Use <see cref="EventChannelUpdated.Cid"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventChannelUpdated> ChannelUpdated;

        /// <summary>
        /// Event raised when a channel is deleted
        ///
        /// Use <see cref="EventChannelDeleted.Cid"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventChannelDeleted> ChannelDeleted;

        /// <summary>
        /// Event raised when a channel is truncated
        ///
        /// Use <see cref="EventChannelTruncated.Cid"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventChannelTruncated> ChannelTruncated;

        /// <summary>
        /// Event raised when a channel is made visible
        ///
        /// Use <see cref="EventChannelVisible.Cid"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventChannelVisible> ChannelVisible;

        /// <summary>
        /// Event raised when a channel is hidden
        ///
        /// Use <see cref="EventChannelHidden.Cid"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventChannelHidden> ChannelHidden;

        /// <summary>
        /// Event raised when a channel member is added to a channel
        ///
        /// Use <see cref="EventMemberAdded.Cid"/> & <see cref="EventMemberAdded.Member"/> to know which channel & member.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventMemberAdded> MemberAdded;

        /// <summary>
        /// Event raised when a channel member is removed from a channel
        ///
        /// Use <see cref="EventMemberRemoved.Cid"/> & <see cref="EventMemberRemoved.Member"/> to know which channel & member.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventMemberRemoved> MemberRemoved;

        /// <summary>
        /// Event raised when a channel member is updated (e.g. promoted to moderator, accepted/rejected the invite, etc.)
        ///
        /// Use <see cref="EventMemberUpdated.Cid"/> & <see cref="EventMemberUpdated.Member"/> to know which channel & member.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventMemberUpdated> MemberUpdated;

        /// <summary>
        /// Event raised when a user status changes (e.g online, offline, away, etc.)
        ///
        /// Use <see cref="EventUserPresenceChanged.User"/> to know which user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventUserPresenceChanged> UserPresenceChanged;

        /// <summary>
        /// Event raised when a user is updated
        ///
        /// Use <see cref="EventUserUpdated.User"/> to know which user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventUserUpdated> UserUpdated;

        /// <summary>
        /// Event raised when a user is deleted
        ///
        /// Use <see cref="EventUserDeleted.User"/> to know which user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventUserDeleted> UserDeleted;

        /// <summary>
        /// Event raised when a user is banned
        ///
        /// Use <see cref="EventUserBanned.User"/> to know which user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventUserBanned> UserBanned;

        /// <summary>
        /// Event raised when a user ban is lifted
        ///
        /// Use <see cref="EventUserUnbanned.User"/> to know which user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventUserUnbanned> UserUnbanned;

        /// <summary>
        /// Event raised when a user starts watching a channel
        ///
        /// Use <see cref="EventUserWatchingStart.Cid"/> & <see cref="EventUserWatchingStart.User"/> to know which channel & user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventUserWatchingStart> UserWatchingStart;

        /// <summary>
        /// Event raised when a user stops watching a channel
        ///
        /// Use <see cref="EventUserWatchingStop.Cid"/> & <see cref="EventUserWatchingStop.User"/> to know which channel & user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventUserWatchingStop> UserWatchingStop;

        /// <summary>
        /// Event raised when a reaction has been added to the message of a watched channel.
        ///
        /// Use <see cref="EventReactionNew.Cid"/> & <see cref="EventReactionNew.Message"/> to know which channel & message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventReactionNew> ReactionReceived;

        /// <summary>
        /// Event raised when a reaction has been updated in the message of a watched channel.
        ///
        /// Use <see cref="EventReactionNew.Cid"/> & <see cref="EventReactionNew.Message"/> to know which channel & message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventReactionUpdated> ReactionUpdated;

        /// <summary>
        /// Event raised when a reaction has been deleted from the message of a watched channel.
        ///
        /// Use <see cref="EventReactionNew.Cid"/> & <see cref="EventReactionNew.Message"/> to know which channel & message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventReactionDeleted> ReactionDeleted;

        /// <summary>
        /// Notification Event raised when the total count of unread messages (across all channels the user is a member) changes.
        ///
        /// Use <see cref="EventNotificationMarkRead.Cid"/> & <see cref="EventNotificationMarkRead.Channel"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationMarkRead> NotificationMarkRead;

        /// <summary>
        /// Notification Event raised when a new message is sent to channel the user is member of.
        ///
        /// Notifications are sent to all channel members regardless of whether they're actively watching this channel.
        /// Use <see cref="EventNotificationMessageNew.Cid"/> & <see cref="EventNotificationMessageNew.Message"/> to know which channel & message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationMessageNew> NotificationMessageReceived;

        /// <summary>
        /// Event raised when a user in a channel started typing
        ///
        /// Use <see cref="EventTypingStart.Cid"/> & <see cref="EventTypingStart.User"/> to know which channel & user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventTypingStart> TypingStarted;

        /// <summary>
        /// Event raised when a user in a channel stopped typing
        ///
        /// Use <see cref="EventTypingStop.Cid"/> & <see cref="EventTypingStop.User"/> to know which channel & user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventTypingStop> TypingStopped;

        /// <summary>
        /// Notification Event raised when channel mutes are updated for local user.
        ///
        /// Notifications are sent to all channel members regardless of whether they're actively watching this channel.
        /// Use <see cref="EventNotificationChannelMutesUpdated.Me.ChannelMutes"/> to get info on which channels are muted
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationChannelMutesUpdated> NotificationChannelMutesUpdated;

        /// <summary>
        /// Notification Event raised when user mutes are updated
        ///
        /// Notifications are sent to all channel members regardless of whether they're actively watching this channel.
        /// Use <see cref="EventNotificationMutesUpdated.Me.Mutes"/> to get info on which users are muted
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationMutesUpdated> NotificationMutesUpdated;

        /// <summary>
        /// Notification Event raised when channel is deleted
        ///
        /// Notifications are sent to all channel members regardless of whether they're actively watching this channel.
        /// Use <see cref="EventNotificationChannelDeleted.Cid"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationChannelDeleted> NotificationChannelDeleted;

        /// <summary>
        /// Notification Event raised when channel is truncated
        ///
        /// Notifications are sent to all channel members regardless of whether they're actively watching this channel.
        /// Use <see cref="EventNotificationChannelTruncated.Cid"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationChannelTruncated> NotificationChannelTruncated;

        /// <summary>
        /// Notification Event raised when a member is added to a channel
        ///
        /// Notifications are sent to all channel members regardless of whether they're actively watching this channel.
        /// Use <see cref="EventNotificationAddedToChannel.Cid"/> & <see cref="EventNotificationAddedToChannel.Member"/> to know which channel & member.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationAddedToChannel> NotificationAddedToChannel;

        /// <summary>
        /// Notification Event raised when a member is removed from a channel
        ///
        /// Notifications are sent to all channel members regardless of whether they're actively watching this channel.
        /// Use <see cref="EventNotificationRemovedFromChannel.Cid"/> & <see cref="EventNotificationRemovedFromChannel.Member"/> to know which channel & member.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationRemovedFromChannel> NotificationRemovedFromChannel;

        /// <summary>
        /// Notification Event raised when the user is invited to join a channel
        ///
        /// Notifications are sent to all channel members regardless of whether they're actively watching this channel.
        /// Use <see cref="EventNotificationInvited.Channel"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationInvited> NotificationInvited;

        /// <summary>
        /// Notification Event raised when the user accepts an invite
        ///
        /// Notifications are sent to all channel members regardless of whether they're actively watching this channel.
        /// Use <see cref="EventNotificationInviteAccepted.Channel"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationInviteAccepted> NotificationInviteAccepted;

        /// <summary>
        /// Notification Event raised when the user rejects an invite
        ///
        /// Notifications are sent to all channel members regardless of whether they're actively watching this channel.
        /// Use <see cref="EventNotificationInviteRejected.Channel"/> to know which channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity</remarks>
        event Action<EventNotificationInviteRejected> NotificationInviteRejected;
    }
}