using System;
using System.Collections.Generic;

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
        
        protected FieldFilterRule InternalEqualsTo(int value)
            => new FieldFilterRule(FieldName, QueryOperatorType.Equals, value);
        
        protected FieldFilterRule InternalEqualsTo(DateTime value)
            => new FieldFilterRule(FieldName, QueryOperatorType.Equals, value);
        
        protected FieldFilterRule InternalEqualsTo(DateTimeOffset value)
            => new FieldFilterRule(FieldName, QueryOperatorType.Equals, value);

        protected FieldFilterRule InternalIn(IEnumerable<string> values)
            => new FieldFilterRule(FieldName, QueryOperatorType.In, values);
        
        protected FieldFilterRule InternalIn(IEnumerable<DateTime> values)
            => new FieldFilterRule(FieldName, QueryOperatorType.In, values);
        
        protected FieldFilterRule InternalIn(IEnumerable<DateTimeOffset> values)
            => new FieldFilterRule(FieldName, QueryOperatorType.In, values);
        
        protected FieldFilterRule InternalGreaterThan(DateTime value)
            => new FieldFilterRule(FieldName, QueryOperatorType.GreaterThan, value);
        
        protected FieldFilterRule InternalGreaterThan(DateTimeOffset value)
            => new FieldFilterRule(FieldName, QueryOperatorType.GreaterThan, value);
        
        protected FieldFilterRule InternalGreaterThan(int value)
            => new FieldFilterRule(FieldName, QueryOperatorType.GreaterThan, value);
        
        protected FieldFilterRule InternalGreaterThan(string value)
            => new FieldFilterRule(FieldName, QueryOperatorType.GreaterThan, value);
        
        protected FieldFilterRule InternalGreaterThanOrEquals(DateTime dateTime)
            => new FieldFilterRule(FieldName, QueryOperatorType.GreaterThanOrEquals, dateTime);
        
        protected FieldFilterRule InternalGreaterThanOrEquals(DateTimeOffset value)
            => new FieldFilterRule(FieldName, QueryOperatorType.GreaterThanOrEquals, value);
        
        protected FieldFilterRule InternalGreaterThanOrEquals(int value)
            => new FieldFilterRule(FieldName, QueryOperatorType.GreaterThanOrEquals, value);
        
        protected FieldFilterRule InternalGreaterThanOrEquals(string value)
            => new FieldFilterRule(FieldName, QueryOperatorType.GreaterThanOrEquals, value);
        
        protected FieldFilterRule InternalLessThan(DateTime value)
            => new FieldFilterRule(FieldName, QueryOperatorType.LessThan, value);
        
        protected FieldFilterRule InternalLessThan(DateTimeOffset value)
            => new FieldFilterRule(FieldName, QueryOperatorType.LessThan, value);
        
        protected FieldFilterRule InternalLessThan(int value)
            => new FieldFilterRule(FieldName, QueryOperatorType.LessThan, value);
        
        protected FieldFilterRule InternalLessThan(string value)
            => new FieldFilterRule(FieldName, QueryOperatorType.LessThan, value);
        
        protected FieldFilterRule InternalLessThanOrEquals(DateTime value)
            => new FieldFilterRule(FieldName, QueryOperatorType.LessThanOrEquals, value);
        
        protected FieldFilterRule InternalLessThanOrEquals(DateTimeOffset value)
            => new FieldFilterRule(FieldName, QueryOperatorType.LessThanOrEquals, value);
        
        protected FieldFilterRule InternalLessThanOrEquals(int value)
            => new FieldFilterRule(FieldName, QueryOperatorType.LessThanOrEquals, value);
        
        protected FieldFilterRule InternalLessThanOrEquals(string value)
            => new FieldFilterRule(FieldName, QueryOperatorType.LessThanOrEquals, value);

        protected FieldFilterRule InternalAutocomplete(string value)
            => new FieldFilterRule(FieldName, QueryOperatorType.Autocomplete, value);
        
        protected FieldFilterRule InternalContains(string value)
            => new FieldFilterRule(FieldName, QueryOperatorType.Contains, value);
    }
}