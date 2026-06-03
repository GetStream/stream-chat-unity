using System.Collections.Generic;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by message type (<see cref="IStreamMessage.Type"/>).
    /// Common values: <c>regular</c>, <c>system</c>, <c>deleted</c>, <c>reply</c>, <c>ephemeral</c>.
    /// </summary>
    public sealed class MessageFieldType : BaseFieldToFilter
    {
        public override string FieldName => "type";

        public FieldFilterRule EqualsTo(string messageType) => InternalEqualsTo(messageType);

        public FieldFilterRule In(IEnumerable<string> messageTypes) => InternalIn(messageTypes);

        public FieldFilterRule In(params string[] messageTypes) => InternalIn(messageTypes);
    }
}
