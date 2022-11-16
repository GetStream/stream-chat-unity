using System;
using System.Collections.Generic;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.State
{
    internal static class IRepositoryExt
    {
        /// <summary>
        /// Clear target list and replace with items created or updated from DTO collection
        /// </summary>
        public static void TryReplaceTrackedObjects<TTracked, TDto>(this IList<TTracked> target, IEnumerable<TDto> dtos,
            ICacheRepository<TTracked> repository)
            where TTracked : class, IStreamStatefulModel, IUpdateableFrom<TDto, TTracked>
        {
            if (target == null)
            {
                throw new ArgumentException(nameof(target));
            }

            if (dtos == null)
            {
                return;
            }

            target.Clear();

            foreach (var dto in dtos)
            {
                var trackedItem = repository.CreateOrUpdate<TTracked, TDto>(dto, out _);
                target.Add(trackedItem);
            }
        }

        /// <summary>
        /// Clear target list and replace with items created or updated from DTO collection
        /// </summary>
        public static void TryAppendUniqueTrackedObjects<TTracked, TDto>(this IList<TTracked> target, IEnumerable<TDto> dtos,
            ICacheRepository<TTracked> repository)
            where TTracked : class, IStreamStatefulModel, IUpdateableFrom<TDto, TTracked>
        {
            if (target == null)
            {
                throw new ArgumentException(nameof(target));
            }

            if (dtos == null)
            {
                return;
            }

            _uniqueElements.Clear();

            foreach (var t in target)
            {
                _uniqueElements.Add(t);
            }

            foreach (var dto in dtos)
            {
                var trackedItem = repository.CreateOrUpdate<TTracked, TDto>(dto, out _);

                if (_uniqueElements.Add(trackedItem))
                {
                    target.Add(trackedItem);
                }
            }
        }

        private static readonly HashSet<object> _uniqueElements = new HashSet<object>();
    }
}