using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filters for <see cref="IStreamMessage"/> conditions used by
    /// <see cref="IStreamChatClient.SearchMessagesAsync"/>.
    ///
    /// These rules go into <see cref="Requests.StreamSearchMessagesRequest.MessageFilter"/>
    /// and are applied to messages within the channels matched by
    /// <see cref="Requests.StreamSearchMessagesRequest.ChannelFilter"/>.
    /// </summary>
    public static class MessageFilter
    {
        /// <inheritdoc cref="MessageFieldText"/>
        public static MessageFieldText Text { get; } = new MessageFieldText();

        /// <inheritdoc cref="MessageFieldUserId"/>
        public static MessageFieldUserId UserId { get; } = new MessageFieldUserId();

        /// <inheritdoc cref="MessageFieldType"/>
        public static MessageFieldType Type { get; } = new MessageFieldType();

        /// <inheritdoc cref="MessageFieldCreatedAt"/>
        public static MessageFieldCreatedAt CreatedAt { get; } = new MessageFieldCreatedAt();

        /// <inheritdoc cref="MessageFieldUpdatedAt"/>
        public static MessageFieldUpdatedAt UpdatedAt { get; } = new MessageFieldUpdatedAt();

        /// <inheritdoc cref="MessageFieldParentId"/>
        public static MessageFieldParentId ParentId { get; } = new MessageFieldParentId();

        /// <inheritdoc cref="MessageFieldPinned"/>
        public static MessageFieldPinned Pinned { get; } = new MessageFieldPinned();

        /// <inheritdoc cref="MessageFieldSilent"/>
        public static MessageFieldSilent Silent { get; } = new MessageFieldSilent();

        /// <inheritdoc cref="MessageFieldMentionedUserId"/>
        public static MessageFieldMentionedUserId MentionedUserId { get; } = new MessageFieldMentionedUserId();

        /// <inheritdoc cref="MessageFieldThreadParticipantId"/>
        public static MessageFieldThreadParticipantId ThreadParticipantId { get; } = new MessageFieldThreadParticipantId();

        /// <inheritdoc cref="MessageFieldAttachmentType"/>
        public static MessageFieldAttachmentType AttachmentType { get; } = new MessageFieldAttachmentType();

        /// <inheritdoc cref="MessageFieldReactionType"/>
        public static MessageFieldReactionType ReactionType { get; } = new MessageFieldReactionType();

        /// <inheritdoc cref="MessageFieldPollId"/>
        public static MessageFieldPollId PollId { get; } = new MessageFieldPollId();

        /// <inheritdoc cref="MessageFieldShowInChannel"/>
        public static MessageFieldShowInChannel ShowInChannel { get; } = new MessageFieldShowInChannel();

        /// <inheritdoc cref="MessageFieldCustom"/>
        public static MessageFieldCustom Custom(string customFieldName) => new MessageFieldCustom(customFieldName);
    }
}
