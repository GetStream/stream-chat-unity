namespace StreamChat.Core
{
    /// <summary>
    /// Event types that can be received from WebSocket connection
    /// </summary>
    internal static class WSEventType
    {
        public const string UserPresenceChanged = "user.presence.changed";
        public const string UserWatchingStart = "user.watching.start";
        public const string UserWatchingStop = "user.watching.stop";
        public const string UserUpdated = "user.updated";
        public const string UserBanned = "user.banned";
        public const string UserDeleted = "user.deleted";
        public const string UserUnbanned = "user.unbanned";
        public const string TypingStart = "typing.start";
        public const string TypingStop = "typing.stop";
        public const string MessageNew = "message.new";
        public const string MessageUpdated = "message.updated";
        public const string MessageDeleted = "message.deleted";
        public const string MessageRead = "message.read";
        public const string ReactionNew = "reaction.new";
        public const string ReactionDeleted = "reaction.deleted";
        public const string ReactionUpdated = "reaction.updated";
        public const string MemberAdded = "member.added";
        public const string MemberRemoved = "member.removed";
        public const string MemberUpdated = "member.updated";
        public const string ChannelUpdated = "channel.updated";
        public const string ChannelHidden = "channel.hidden";
        public const string ChannelDeleted = "channel.deleted";
        public const string ChannelVisible = "channel.visible";
        public const string ChannelTruncated = "channel.truncated";
        public const string HealthCheck = "health.check";
        public const string NotificationMessageNew = "notification.message_new";
        public const string NotificationChannelTruncated = "notification.channel_truncated";
        public const string NotificationChannelDeleted = "notification.channel_deleted";
        public const string NotificationMarkRead = "notification.mark_read";
        public const string NotificationInvited = "notification.invited";
        public const string NotificationInviteAccepted = "notification.invite_accepted";
        public const string NotificationInviteRejected = "notification.invite_rejected";
        public const string NotificationAddedToChannel = "notification.added_to_channel";
        public const string NotificationRemovedFromChannel = "notification.removed_from_channel";
        public const string NotificationMutesUpdated = "notification.mutes_updated";
        public const string NotificationChannelMutesUpdated = "notification.channel_mutes_updated";
    }
}