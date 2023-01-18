using System;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.LastMessageAt"/>
    /// </summary>
    public class ChannelFieldLastMessageAt : BaseFieldToFilter
    {
        public override string FieldName => "LAST_MESSAGE_AT";
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.LastMessageAt"/> is equal to provided one
        /// </summary>
        public FieldFilterRule EqualsTo(DateTime dateTime) => InternalEqualsTo(dateTime);
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.LastMessageAt"/> is equal to provided one
        /// </summary>
        public FieldFilterRule EqualsTo(DateTimeOffset dateTime) => InternalEqualsTo(dateTime);

        public FieldFilterRule LessThanOrEquals(DateTime dateTime) => InternalLessThanOrEquals(dateTime);
        
        public FieldFilterRule LessThanOrEquals(DateTimeOffset dateTimeOffset)
            => InternalLessThanOrEquals(dateTimeOffset);
    }
}