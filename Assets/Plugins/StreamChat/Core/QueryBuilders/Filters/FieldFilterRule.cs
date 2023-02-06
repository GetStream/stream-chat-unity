using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StreamChat.Core.QueryBuilders.Filters
{
    public sealed class FieldFilterRule : IFieldFilterRule
    {
        public string Field { get; }
        public QueryOperatorType OperatorType { get; }
        public object Value { get; }

        public FieldFilterRule(string field, QueryOperatorType operatorType, bool value)
        {
            Field = field;
            OperatorType = operatorType;
            Value = value;
        }

        public FieldFilterRule(string field, QueryOperatorType operatorType, string value)
        {
            Field = field;
            OperatorType = operatorType;
            Value = value;
        }
        
        public FieldFilterRule(string field, QueryOperatorType operatorType, int value)
        {
            Field = field;
            OperatorType = operatorType;
            Value = value;
        }

        public FieldFilterRule(string field, QueryOperatorType operatorType, DateTime value)
        {
            Field = field;
            OperatorType = operatorType;
            Value = ToRfc3339String(value);
        }

        public FieldFilterRule(string field, QueryOperatorType operatorType, DateTimeOffset value)
        {
            Field = field;
            OperatorType = operatorType;
            Value = ToRfc3339String(value);
        }

        public FieldFilterRule(string field, QueryOperatorType operatorType, IEnumerable<string> value)
        {
            Field = field;
            OperatorType = operatorType;
            Value = value.ToArray();
        }
        
        public FieldFilterRule(string field, QueryOperatorType operatorType, IEnumerable<DateTime> value)
        {
            Field = field;
            OperatorType = operatorType;
            Value = value.ToArray();
        }
        
        public FieldFilterRule(string field, QueryOperatorType operatorType, IEnumerable<DateTimeOffset> value)
        {
            Field = field;
            OperatorType = operatorType;
            Value = value.ToArray();
        }

        //StreamTodo: research how to reduce allocation here
        public KeyValuePair<string, object> GenerateFilterEntry()
            => new KeyValuePair<string, object>
            (
                Field, new Dictionary<string, object>
                {
                    {
                        OperatorType.ToOperatorKeyword(), Value
                    }
                }
            );
        
        private static string ToRfc3339String(DateTime dateTime)
            => dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
        
        private static string ToRfc3339String(DateTimeOffset dateTimeOffset)
            => dateTimeOffset.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
    }
}