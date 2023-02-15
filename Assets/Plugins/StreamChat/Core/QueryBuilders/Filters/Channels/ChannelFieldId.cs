using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Id"/>. If possible, you should filter by <see cref="IStreamChannel.Cid"/> which is much faster
    /// </summary>
    public sealed class ChannelFieldId : BaseFieldToFilter
    {
        public override string FieldName => "id";
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Id"/> is EQUAL to provided channel Id. If possible, you should filter by <see cref="IStreamChannel.Cid"/> which is much faster
        /// </summary>
        public FieldFilterRule EqualsTo(string channelCid) => InternalEqualsTo(channelCid);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Id"/> is EQUAL to ANY of provided channel Id. If possible, you should filter by <see cref="IStreamChannel.Cid"/> which is much faster
        /// </summary>
        public FieldFilterRule In(IEnumerable<string> channelCids) => InternalIn(channelCids);
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Id"/> is EQUAL to ANY of provided channel Id. If possible, you should filter by <see cref="IStreamChannel.Cid"/> which is much faster
        /// </summary>
        public FieldFilterRule In(params string[] channelCids) => InternalIn(channelCids);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Id"/> is EQUAL to ANY of the provided channels Id. If possible, you should filter by <see cref="IStreamChannel.Cid"/> which is much faster
        /// </summary>
        public FieldFilterRule In(IEnumerable<IStreamChannel> channels)
            => InternalIn(channels.Select(_ => _.Id));

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Id"/> is EQUAL to ANY of the provided channels Id. If possible, you should filter by <see cref="IStreamChannel.Cid"/> which is much faster
        /// </summary>
        public FieldFilterRule In(params IStreamChannel[] channels)
            => InternalIn(channels.Select(_ => _.Id));
    }
}