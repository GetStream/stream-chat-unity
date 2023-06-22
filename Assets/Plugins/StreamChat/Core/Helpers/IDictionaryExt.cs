using System.Collections.Generic;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// Extensions for <see cref="IDictionary{TKey,TValue}"/>
    /// </summary>
    internal static class IDictionaryExt
    {
        public static Dictionary<TKey, TDto> TrySaveToDtoDictionary<TDto, TSource, TKey>(this Dictionary<TKey, TSource> source)
            where TSource : ISavableTo<TDto>
        {
            if (source == null)
            {
                return null;
            }

            var dict = new Dictionary<TKey, TDto>();

            foreach (var sourceKeyValue in source)
            {
                if (sourceKeyValue.Value == null)
                {
                    continue;
                }

                dict.Add(sourceKeyValue.Key, sourceKeyValue.Value.SaveToDto());
            }

            return dict;
        }

        public static Dictionary<TKey, TSource> TryLoadFromDtoDictionary<TDto, TSource, TKey>(
            this Dictionary<TKey, TSource> _, Dictionary<TKey, TDto> dtos)
            where TSource : ILoadableFrom<TDto, TSource>, new()
        {
            if (dtos == null)
            {
                return null;
            }

            var dict = new Dictionary<TKey, TSource>();

            foreach (var sourceKeyValue in dtos)
            {
                if (sourceKeyValue.Value == null)
                {
                    continue;
                }

                dict.Add(sourceKeyValue.Key, new TSource().LoadFromDto(sourceKeyValue.Value));
            }

            return dict;
        }
    }
}