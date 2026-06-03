using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by <see cref="IStreamMessage.ShowInChannel"/>. Relevant for thread replies that are also
    /// shown in the parent channel feed.
    /// </summary>
    public sealed class MessageFieldShowInChannel : BaseFieldToFilter
    {
        public override string FieldName => "show_in_channel";

        public FieldFilterRule EqualsTo(bool showInChannel) => InternalEqualsTo(showInChannel);
    }
}
