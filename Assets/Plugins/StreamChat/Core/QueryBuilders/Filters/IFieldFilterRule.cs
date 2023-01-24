using System.Collections.Generic;

namespace StreamChat.Core.QueryBuilders.Filters
{
    /// <summary>
    /// This filter applies a rule that will be matched against specific field in a query
    /// E.g. "ID equals 21" - checks if field `ID` is equal to `21` value 
    /// </summary>
    public interface IFieldFilterRule
    {
        string Field { get; }

        KeyValuePair<string, object> GenerateFilterEntry();
    }
}