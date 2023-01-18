using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamUser.Id"/> of a user who created the <see cref="IStreamChannel"/>
    /// </summary>
    public class ChannelFieldCreatedById : BaseFieldToFilter
    {
        public override string FieldName => "CREATED_BY_ID";
        
        public FieldFilterRule EqualsTo(string id) => InternalEqualsTo(id);
        
        public FieldFilterRule EqualsTo(IStreamUser user) => InternalEqualsTo(user.Id);
    }
}