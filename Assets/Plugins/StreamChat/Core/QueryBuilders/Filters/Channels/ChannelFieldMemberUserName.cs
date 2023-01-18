using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamUser.Name"/> who is a member of 
    /// </summary>
    public class ChannelFieldMemberUserName : BaseFieldToFilter
    {
        public override string FieldName => "MEMBER.USER.NAME";
        
        public FieldFilterRule EqualsTo(string userName) => InternalEqualsTo(userName);
        
        public FieldFilterRule EqualsTo(IStreamUser user) => InternalEqualsTo(user.Name);
        
        public FieldFilterRule Autocomplete(string userName) => InternalAutocomplete(userName);
        
        public FieldFilterRule Autocomplete(IStreamUser user) => InternalAutocomplete(user.Name);
    }
}