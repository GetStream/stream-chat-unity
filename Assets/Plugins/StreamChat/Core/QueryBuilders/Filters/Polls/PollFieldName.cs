using System.Collections.Generic;

namespace StreamChat.Core.QueryBuilders.Filters.Polls
{
    /// <summary>
    /// Filter by Poll Name
    /// </summary>
    public sealed class PollFieldName : BaseFieldToFilter
    {
        public override string FieldName => "name";

        /// <summary>
        /// Return only polls where Name is EQUAL to provided name
        /// </summary>
        public FieldFilterRule EqualsTo(string name) => InternalEqualsTo(name);

        /// <summary>
        /// Return only polls where Name is EQUAL to ANY of provided names
        /// </summary>
        public FieldFilterRule In(IEnumerable<string> names) => InternalIn(names);

        /// <summary>
        /// Return only polls where Name is EQUAL to ANY of provided names
        /// </summary>
        public FieldFilterRule In(params string[] names) => InternalIn(names);

        /// <summary>
        /// Return only polls where Name CONTAINS the provided substring (case-insensitive)
        /// </summary>
        public FieldFilterRule Autocomplete(string substring) => InternalAutocomplete(substring);
    }
}

