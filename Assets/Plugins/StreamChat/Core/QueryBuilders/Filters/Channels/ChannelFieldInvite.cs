using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Invite"/>
    /// </summary>
    public class ChannelFieldInvite : BaseFieldToFilter
    {
        public override string FieldName => "INVITE";
        
        public FieldFilterRule EqualsTo(bool isInvited) => InternalEqualsTo(isInvited);
    }
}