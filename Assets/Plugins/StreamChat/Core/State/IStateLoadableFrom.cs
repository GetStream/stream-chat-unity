using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace StreamChat.Core.State
{
    /// <summary>
    /// Supports loading object from DTO of a given type
    /// </summary>
    /// <typeparam name="TDto">DTO type</typeparam>
    /// <typeparam name="TDomain">Domain object type</typeparam>
    internal interface IStateLoadableFrom<in TDto, out TDomain>
        where TDomain : IStateLoadableFrom<TDto, TDomain>
    {
        TDomain LoadFromDto(TDto dto, ICache cache);
    }

    /// <summary>
    /// Extensions for <see cref="ILoadableFrom{TDto,TDomain}"/>
    /// </summary>
    internal static class IStateLoadableFrom
    {
        public static TDomain TryLoadFromDto<TDto, TDomain>(this IStateLoadableFrom<TDto, TDomain> loadable, TDto dto, ICache cache)
            where TDomain : class, IStateLoadableFrom<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return null;
            }

            return new TDomain().LoadFromDto(dto, cache);
        }

        [Pure]
        public static List<TSource> TryLoadFromDtoCollection<TDto, TSource>(this List<TSource> _, List<TDto> dtos, ICache cache)
            where TSource : IStateLoadableFrom<TDto, TSource>, new()
        {
            if (dtos == null)
            {
                return null;
            }

            var items = new List<TSource>(dtos.Count);

            foreach (var dto in dtos)
            {
                items.Add(new TSource().LoadFromDto(dto, cache));
            }

            return items;
        }

        public static void TryReplaceCollectionFromDto<TDto, TSource>(this List<TSource> target, List<TDto> dtos, ICache cache)
            where TSource : IStateLoadableFrom<TDto, TSource>, new()
        {
            if (typeof(TSource) is IStreamTrackedObject)
            {
                throw new InvalidOperationException("This method should not be used for tracked objects");
            }

            if (dtos == null)
            {
                return;
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.Clear();

            foreach (var dto in dtos)
            {
                target.Add(new TSource().LoadFromDto(dto, cache));
            }
        }
    }



}