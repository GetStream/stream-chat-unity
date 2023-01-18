using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StreamChat.Core.QueryBuilders.Filters
{
    /// <summary>
    /// Base class for field filters
    /// </summary>
    public abstract class BaseFieldToFilter : IFieldToFilter
    {
        public abstract string FieldName { get; }

        protected FieldFilterRule InternalEqualsTo(bool value)
            => new FieldFilterRule(FieldName, QueryOperatorType.Equals, value);
        
        protected FieldFilterRule InternalEqualsTo(string value)
            => new FieldFilterRule(FieldName, QueryOperatorType.Equals, value);
        
        protected FieldFilterRule InternalEqualsTo(DateTime value)
            => new FieldFilterRule(FieldName, QueryOperatorType.Equals, value);
        
        protected FieldFilterRule InternalEqualsTo(DateTimeOffset value)
            => new FieldFilterRule(FieldName, QueryOperatorType.Equals, value);

        protected FieldFilterRule InternalIn(IEnumerable<string> values)
            => new FieldFilterRule(FieldName, QueryOperatorType.In, values);
        
        protected FieldFilterRule InternalLessThanOrEquals(DateTime dateTime)
            => new FieldFilterRule(FieldName, QueryOperatorType.LessThanOrEquals, dateTime);
        
        protected FieldFilterRule InternalLessThanOrEquals(DateTimeOffset dateTimeOffset)
            => new FieldFilterRule(FieldName, QueryOperatorType.LessThanOrEquals, dateTimeOffset);

        protected FieldFilterRule InternalAutocomplete(string value)
            => new FieldFilterRule(FieldName, QueryOperatorType.Autocomplete, value);
    }
}