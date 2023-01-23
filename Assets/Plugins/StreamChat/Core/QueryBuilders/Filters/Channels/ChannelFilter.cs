using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filters for <see cref="IStreamChannel"/> query filters in <see cref="IStreamChatClient.QueryChannelsAsync"/>
    /// </summary>
    public static class ChannelFilter
    {
        /// <inheritdoc cref="ChannelFieldFrozen"/>
        public static ChannelFieldFrozen Frozen { get; } = new ChannelFieldFrozen();

        /// <inheritdoc cref="ChannelFieldType"/>
        public static ChannelFieldType Type { get; } = new ChannelFieldType();

        /// <inheritdoc cref="ChannelFieldId"/>
        public static ChannelFieldId Id { get; } = new ChannelFieldId();

        /// <inheritdoc cref="ChannelFieldCid"/>
        public static ChannelFieldCid Cid { get; } = new ChannelFieldCid();

        /// <inheritdoc cref="ChannelFieldMembers"/>
        public static ChannelFieldMembers Members { get; } = new ChannelFieldMembers();

        /// <inheritdoc cref="ChannelFieldInvite"/>
        public static ChannelFieldInvite Invite { get; } = new ChannelFieldInvite();

        /// <inheritdoc cref="ChannelFieldJoined"/>
        public static ChannelFieldJoined Joined { get; } = new ChannelFieldJoined();

        /// <inheritdoc cref="ChannelFieldMuted"/>
        public static ChannelFieldMuted Muted { get; } = new ChannelFieldMuted();

        /// <inheritdoc cref="ChannelFieldMemberUserName"/>
        public static ChannelFieldMemberUserName MemberUserName { get; } = new ChannelFieldMemberUserName();

        /// <inheritdoc cref="ChannelFieldCreatedById"/>
        public static ChannelFieldCreatedById CreatedById { get; } = new ChannelFieldCreatedById();

        /// <inheritdoc cref="ChannelFieldHidden"/>
        public static ChannelFieldHidden Hidden { get; } = new ChannelFieldHidden();

        /// <inheritdoc cref="ChannelFieldLastMessageAt"/>
        public static ChannelFieldLastMessageAt LastMessageAt { get; } = new ChannelFieldLastMessageAt();

        /// <inheritdoc cref="ChannelFieldMemberCount"/>
        public static ChannelFieldMemberCount MembersCount { get; } = new ChannelFieldMemberCount();

        /// <inheritdoc cref="ChannelFieldCreatedAt"/>
        public static ChannelFieldCreatedAt CreatedAt { get; } = new ChannelFieldCreatedAt();

        /// <inheritdoc cref="ChannelFieldUpdatedAt"/>
        public static ChannelFieldUpdatedAt UpdatedAt { get; } = new ChannelFieldUpdatedAt();

        /// <inheritdoc cref="ChannelFieldTeam"/>
        public static ChannelFieldTeam Team { get; } = new ChannelFieldTeam();

        /// <inheritdoc cref="ChannelFieldDisabled"/>
        public static ChannelFieldDisabled Disabled { get; } = new ChannelFieldDisabled();
        
        /// <inheritdoc cref="ChannelFieldCustom"/>
        public static ChannelFieldCustom Custom(string customFieldName) => new ChannelFieldCustom(customFieldName);
    }
}