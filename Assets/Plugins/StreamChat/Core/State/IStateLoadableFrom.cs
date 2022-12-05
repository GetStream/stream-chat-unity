using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using StreamChat.Core.State.Caches;

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
        
        public static TDomain LoadFromDto<TDto, TDomain>(this IStateLoadableFrom<TDto, TDomain> loadable, TDto dto, ICache cache)
            where TDomain : class, IStateLoadableFrom<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return null;
            }

            if (loadable == null)
            {
                throw new ArgumentException(nameof(loadable));
            }

            return loadable.LoadFromDto(dto, cache);
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


        /// <summary>
        /// Regular = non tracked objects
        /// </summary>
        public static void TryReplaceRegularObjectsFromDto<TDto, TSource>(this List<TSource> target, List<TDto> dtos, ICache cache)
            where TSource : IStateLoadableFrom<TDto, TSource>, new()
        {
            if (typeof(TSource) is IStreamStatefulModel)
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

        /// <summary>
        /// Values = value types
        /// </summary>
        public static void TryReplaceValuesFromDto<TKey, TValue>(this Dictionary<TKey, TValue> target, Dictionary<TKey, TValue> values)
        {
            if (values == null)
            {
                return;
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.Clear();

            foreach (var keyValuePair in values)
            {
                target.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public static void TryReplaceValuesFromDto(this List<string> target, List<string> values)
            => TryReplaceValuesFromDto<string>(target, values);

        public static void TryReplaceValuesFromDto(this List<int> target, List<int> values)
            => TryReplaceValuesFromDto<int>(target, values);

        private static void TryReplaceValuesFromDto<TValue>(this List<TValue> target, List<TValue> values)
        {
            if (values == null)
            {
                return;
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.Clear();

            foreach (var dto in values)
            {
                target.Add(dto);
            }
        }
    }
}