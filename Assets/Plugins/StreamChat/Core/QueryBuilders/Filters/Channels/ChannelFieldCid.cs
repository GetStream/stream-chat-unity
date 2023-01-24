using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Cid"/>
    /// </summary>
    public sealed class ChannelFieldCid : BaseFieldToFilter
    {
        public override string FieldName => "cid";

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Cid"/> is EQUAL to provided channel Cid
        /// </summary>
        public FieldFilterRule EqualsTo(string channelCid) => InternalEqualsTo(channelCid);
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Cid"/> is EQUAL to provided channel
        /// </summary>
        public FieldFilterRule EqualsTo(IStreamChannel channel) => InternalEqualsTo(channel.Cid);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Cid"/> is EQUAL to ANY of provided channel Cid
        /// </summary>
        public FieldFilterRule In(IEnumerable<string> channelCids) => InternalIn(channelCids);
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Cid"/> is EQUAL to ANY of provided channel Cid
        /// </summary>
        public FieldFilterRule In(params string[] channelCids) => InternalIn(channelCids);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Cid"/> is EQUAL to ANY of the provided channels Cid
        /// </summary>
        public FieldFilterRule In(IEnumerable<IStreamChannel> channels)
            => InternalIn(channels.Select(_ => _.Cid));

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Cid"/> is EQUAL to ANY of the provided channels Cid
        /// </summary>
        public FieldFilterRule In(params IStreamChannel[] channels)
            => InternalIn(channels.Select(_ => _.Cid));
    }
}