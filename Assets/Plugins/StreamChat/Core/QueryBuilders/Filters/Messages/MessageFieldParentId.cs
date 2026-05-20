using System.Collections.Generic;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by <see cref="IStreamMessage.ParentId"/>.
    ///
    /// Typical usage:
    /// <list type="bullet">
    ///   <item><c>Exists(true)</c> - only thread replies.</item>
    ///   <item><c>Exists(false)</c> - only top-level messages.</item>
    ///   <item><c>EqualsTo(parentId)</c> - replies to a specific parent message.</item>
    /// </list>
    /// </summary>
    public sealed class MessageFieldParentId : BaseFieldToFilter
    {
        public override string FieldName => "parent_id";

        public FieldFilterRule EqualsTo(string parentMessageId) => InternalEqualsTo(parentMessageId);

        public FieldFilterRule EqualsTo(IStreamMessage parentMessage) => InternalEqualsTo(parentMessage.Id);

        public FieldFilterRule In(IEnumerable<string> parentMessageIds) => InternalIn(parentMessageIds);

        public FieldFilterRule In(params string[] parentMessageIds) => InternalIn(parentMessageIds);

        /// <summary>
        /// When <c>true</c>, returns only replies (messages whose <c>parent_id</c> is set).
        /// When <c>false</c>, returns only top-level (non-reply) messages.
        /// </summary>
        public FieldFilterRule Exists(bool exists) => InternalExists(exists);
    }
}
