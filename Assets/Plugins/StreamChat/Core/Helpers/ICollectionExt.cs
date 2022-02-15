using System.Collections;
using System.Collections.Generic;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// <see cref="ICollection"/> extensions
    /// </summary>
    internal static class ICollectionExt
    {
        public static ICollection<TDto> TrySaveToDtoCollection<TSource, TDto>(this ICollection<TSource> source)
            where TSource : ISavableTo<TDto>
        {
            if (source == null)
            {
                return null;
            }

            var dtos = new List<TDto>(source.Count);

            foreach (var item in source)
            {
                dtos.Add(item.SaveToDto());
            }

            return dtos;
        }

        public static List<TSource> TryLoadFromDtoCollection<TDto, TSource>(this ICollection<TSource> _,
            ICollection<TDto> dtos)
            where TSource : ILoadableFrom<TDto, TSource>, new()
        {
            if (dtos == null)
            {
                return null;
            }

            var items = new List<TSource>(dtos.Count);

            foreach (var dto in dtos)
            {
                items.Add(new TSource().LoadFromDto(dto));
            }

            return items;
        }
    }
}