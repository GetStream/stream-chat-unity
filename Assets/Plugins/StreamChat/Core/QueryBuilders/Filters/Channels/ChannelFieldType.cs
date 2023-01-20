using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Type"/>
    /// </summary>
    public sealed class ChannelFieldType : BaseFieldToFilter
    {
        public override string FieldName => "type";

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Type"/> state is EQUAL to the provided value
        /// </summary>
        public FieldFilterRule EqualsTo(ChannelType channelType)
            => InternalEqualsTo(channelType.ToString());

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Type"/> is EQUAL to ANY of provided values
        /// </summary>
        public FieldFilterRule In(IEnumerable<ChannelType> channelTypes)
            => InternalIn(channelTypes.Select(_ => _.ToString()));
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Type"/> is EQUAL to ANY of provided values
        /// </summary>
        public FieldFilterRule In(params ChannelType[] channelTypes)
            => InternalIn(channelTypes.Select(_ => _.ToString()));
    }
}