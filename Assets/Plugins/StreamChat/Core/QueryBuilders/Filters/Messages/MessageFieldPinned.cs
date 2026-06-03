using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by <see cref="IStreamMessage.Pinned"/>.
    /// Useful for cross-channel pinned-message searches.
    /// </summary>
    public sealed class MessageFieldPinned : BaseFieldToFilter
    {
        public override string FieldName => "pinned";

        public FieldFilterRule EqualsTo(bool pinned) => InternalEqualsTo(pinned);
    }
}
