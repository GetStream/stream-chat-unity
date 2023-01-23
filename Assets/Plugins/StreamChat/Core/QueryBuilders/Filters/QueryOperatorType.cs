using System;

namespace StreamChat.Core.QueryBuilders.Filters
{
    /// <summary>
    /// Query filter operator type
    /// </summary>
    public enum QueryOperatorType
    {
        Exists,
        Contains,
        And,
        Or,
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEquals,
        LessThan,
        LessThanOrEquals,
        In,
        NotIn,
        Autocomplete,
    }
    
    /// <summary>
    /// Extensions for <see cref="QueryOperatorType"/>
    /// </summary>
    public static class QueryOperatorTypeExt
    {
        public static string ToOperatorKeyword(this QueryOperatorType operatorType)
        {
            switch (operatorType)
            {
                case QueryOperatorType.Exists: return "$exists";
                case QueryOperatorType.Contains: return "$contains";
                case QueryOperatorType.And: return "$and";
                case QueryOperatorType.Or: return "$or";
                case QueryOperatorType.Equals: return "$eq";
                case QueryOperatorType.NotEquals: return "$neq";
                case QueryOperatorType.GreaterThan: return "$gt";
                case QueryOperatorType.GreaterThanOrEquals: return "$gte";
                case QueryOperatorType.LessThan: return "$lt";
                case QueryOperatorType.LessThanOrEquals: return "$lte";
                case QueryOperatorType.In: return "$in";
                case QueryOperatorType.NotIn: return "$nin";
                case QueryOperatorType.Autocomplete: return "$autocomplete";
                default:
                    throw new ArgumentOutOfRangeException(nameof(operatorType), operatorType, null);
            }
        }
    }
}