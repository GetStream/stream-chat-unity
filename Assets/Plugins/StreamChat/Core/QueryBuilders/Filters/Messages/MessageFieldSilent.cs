using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by <see cref="IStreamMessage.Silent"/>.
    /// </summary>
    public sealed class MessageFieldSilent : BaseFieldToFilter
    {
        public override string FieldName => "silent";

        public FieldFilterRule EqualsTo(bool silent) => InternalEqualsTo(silent);
    }
}
