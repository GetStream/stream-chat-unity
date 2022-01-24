using System.Collections;
using System.Collections.Generic;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// <see cref="ICollection"/> extensions
    /// </summary>
    public static class ICollectionExt
    {
        public static ICollection<TDto> TrySaveToDtoCollection<TSource, TDto>(this ICollection<TSource> collection)
            where TSource : ISavableTo<TDto>
        {
            if (collection == null)
            {
                return null;
            }

            var dtos = new List<TDto>(collection.Count);

            foreach (var item in collection)
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