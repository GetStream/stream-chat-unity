using System;
using System.Collections.Generic;
using StreamChat.Core.State;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel"/> custom field. Custom fields can be defined in <see cref="IStreamChatClient.GetOrCreateChannelWithIdAsync"/>,
    /// <see cref="IStreamChatClient.GetOrCreateChannelWithMembersAsync"/>, or <see cref="IStreamChannel.UpdatePartialAsync"/>
    /// </summary>
    public sealed class ChannelFieldCustom : BaseFieldToFilter
    {
        public override string FieldName { get; }

        public ChannelFieldCustom(string customFieldName)
        {
            StreamAsserts.AssertNotNullOrEmpty(customFieldName, nameof(customFieldName));
            FieldName = customFieldName;
        }

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel"/> custom field has value EQUAL to the provided one
        /// </summary>
        public FieldFilterRule EqualsTo(string value) => InternalEqualsTo(value);
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel"/> custom field has value EQUAL to ANY of provided channel Id.
        /// </summary>
        public FieldFilterRule In(IEnumerable<string> values) => InternalIn(values);
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel"/> custom field has value EQUAL to ANY of provided channel Id.
        /// </summary>
        public FieldFilterRule In(params string[] values) => InternalIn(values);
    }
}