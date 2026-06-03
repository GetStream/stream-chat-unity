using System.Collections.Generic;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by the type of an attachment on the message (<c>image</c>, <c>video</c>,
    /// <c>file</c>, <c>audio</c>, <c>giphy</c>, <c>location</c>, or any custom type).
    /// </summary>
    public sealed class MessageFieldAttachmentType : BaseFieldToFilter
    {
        public override string FieldName => "attachments.type";

        public FieldFilterRule EqualsTo(string attachmentType) => InternalEqualsTo(attachmentType);

        public FieldFilterRule Contains(string attachmentType) => InternalContains(attachmentType);

        public FieldFilterRule In(IEnumerable<string> attachmentTypes) => InternalIn(attachmentTypes);

        public FieldFilterRule In(params string[] attachmentTypes) => InternalIn(attachmentTypes);
    }
}
