using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Users
{
    /// <summary>
    /// Filter by <see cref="IStreamUser.Name"/>
    /// </summary>
    public sealed class UserFieldName : BaseFieldToFilter
    {
        public override string FieldName => "name";
        
        /// <summary>
        /// Return only users where <see cref="IStreamUser.Name"/> is EQUAL to the provided value
        /// </summary>
        public FieldFilterRule EqualsTo(string userName) => InternalEqualsTo(userName);
        
        /// <summary>
        /// Return only users where <see cref="IStreamUser.Name"/> is EQUAL to the provided user
        /// </summary>
        public FieldFilterRule EqualsTo(IStreamUser user) => InternalEqualsTo(user.Name);
        
        /// <summary>
        /// Return only users where <see cref="IStreamUser.Name"/> CONTAINS provided phrase anywhere. Example: "An" would match "Anna", "Anatoly", "Annabelle" because they all start with "An"
        /// </summary>
        public FieldFilterRule Autocomplete(string phrase) => InternalAutocomplete(phrase);
    }
}