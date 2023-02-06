using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Users
{
    /// <summary>
    /// Filter by <see cref="IStreamUser.Teams"/>
    /// </summary>
    public sealed class UserFieldTeams : BaseFieldToFilter
    {
        public override string FieldName => "teams";
        
        /// <summary>
        /// Return only users where <see cref="IStreamUser.Teams"/> is EQUAL to the provided value
        /// </summary>
        public FieldFilterRule EqualsTo(string team) => InternalEqualsTo(team);
        
        /// <summary>
        /// Return only users where <see cref="IStreamUser.Name"/> CONTAINS provided phrase anywhere. Example: "An" would match "Anna", "Anatoly", "Annabelle" because they all start with "An"
        /// </summary>
        public FieldFilterRule Contains(string team) => InternalContains(team);
    }
}