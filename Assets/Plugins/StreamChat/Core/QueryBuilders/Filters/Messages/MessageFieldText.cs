using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Messages
{
    /// <summary>
    /// Filter by <see cref="IStreamMessage.Text"/>.
    ///
    /// Note: combining a <c>text</c> rule with
    /// <see cref="Requests.StreamSearchMessagesRequest.Query"/> is rejected by the server
    /// and validated client-side.
    /// </summary>
    public sealed class MessageFieldText : BaseFieldToFilter
    {
        public override string FieldName => "text";

        /// <summary>
        /// Return only messages where <see cref="IStreamMessage.Text"/> is EQUAL to the provided value.
        /// </summary>
        public FieldFilterRule EqualsTo(string text) => InternalEqualsTo(text);

        /// <summary>
        /// Return only messages where <see cref="IStreamMessage.Text"/> CONTAINS the provided phrase.
        /// </summary>
        public FieldFilterRule Contains(string phrase) => InternalContains(phrase);

        /// <summary>
        /// Return only messages where <see cref="IStreamMessage.Text"/> matches the provided autocomplete phrase.
        /// </summary>
        public FieldFilterRule Autocomplete(string phrase) => InternalAutocomplete(phrase);
    }
}
