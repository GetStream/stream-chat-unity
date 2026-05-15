using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Threads
{
    /// <summary>
    /// Filter by Thread <c>channel_cid</c>
    /// </summary>
    public sealed class ThreadFieldChannelCid : BaseFieldToFilter
    {
        public override string FieldName => "channel_cid";

        public FieldFilterRule EqualsTo(string channelCid) => InternalEqualsTo(channelCid);

        public FieldFilterRule EqualsTo(IStreamChannel channel) => InternalEqualsTo(channel.Cid);

        public FieldFilterRule In(IEnumerable<string> channelCids) => InternalIn(channelCids);

        public FieldFilterRule In(params string[] channelCids) => InternalIn(channelCids);

        public FieldFilterRule In(IEnumerable<IStreamChannel> channels) => InternalIn(channels.Select(_ => _.Cid));

        public FieldFilterRule In(params IStreamChannel[] channels) => InternalIn(channels.Select(_ => _.Cid));
    }
}
