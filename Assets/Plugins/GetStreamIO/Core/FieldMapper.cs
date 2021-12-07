using System;
using System.Collections.Generic;

namespace Plugins.GetStreamIO.Core
{
    /// <summary>
    /// Maps typed field key to string name
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class FieldMapper<TKey>
    {
        public string this[TKey key]
        {
            get
            {
                if (_mapping.TryGetValue(key, out var field))
                {
                    return field;
                }

                throw new KeyNotFoundException($"{nameof(key)} of value `{key}` not found");
            }
        }

        public FieldMapper(Action<IDictionary<TKey, string>> ctor)
        {
            ctor(_mapping);
        }

        private readonly IDictionary<TKey, string> _mapping = new Dictionary<TKey, string>();
    }
}