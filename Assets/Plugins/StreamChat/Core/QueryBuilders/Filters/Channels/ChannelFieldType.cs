using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Type"/>
    /// </summary>
    public class ChannelFieldType : BaseFieldToFilter
    {
        public override string FieldName => "type";

        public FieldFilterRule EqualsTo(ChannelType channelType)
            => InternalEqualsTo(channelType.ToString());

        public FieldFilterRule In(IEnumerable<ChannelType> channelTypes)
            => InternalIn(channelTypes.Select(_ => _.ToString()));
        
        public FieldFilterRule In(params ChannelType[] channelTypes)
            => InternalIn(channelTypes.Select(_ => _.ToString()));
    }
}