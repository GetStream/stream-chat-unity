using System.Collections.Generic;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// Extensions for <see cref="IDictionary{TKey,TValue}"/>
    /// </summary>
    internal static class IDictionaryExt
    {
        public static IDictionary<TKey, TDto> TrySaveToDtoDictionary<TDto, TSource, TKey>(this IDictionary<TKey, TSource> source)
            where TSource : ISavableTo<TDto>
        {
            if (source == null)
            {
                return null;
            }

            var dict = new Dictionary<TKey, TDto>();

            foreach (var sourceKeyValue in source)
            {
                dict.Add(sourceKeyValue.Key, sourceKeyValue.Value.SaveToDto());
            }

            return dict;
        }

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