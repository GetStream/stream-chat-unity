using System.Collections.Generic;
using StreamChat.Core;

namespace Plugins.StreamChat.Core.Helpers
{
    /// <summary>
    /// Extensions for <see cref="IDictionary{TKey,TValue}"/>
    /// </summary>
    public static class IDictionaryExt
    {
        public static IDictionary<TKey, TSource> TryLoadFromDtoDictionary<TDto, TSource, TKey>(
            this IDictionary<TKey, TSource> _, IDictionary<TKey, TDto> dtos)
            where TSource : ILoadableFrom<TDto, TSource>, new()
        {
            if (dtos == null)
            {
                return null;
            }

            var dict = new Dictionary<TKey, TSource>();

            foreach (var sourceKeyValue in dtos)
            {
                dict.Add(sourceKeyValue.Key, new TSource().LoadFromDto(sourceKeyValue.Value));
            }

            return dict;
        }
    }
}