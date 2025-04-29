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
                try
                {
                    target.Add(trackedItem);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Append target list with items created from DTO enumeration. Fails silently if target or source are null.
        /// </summary>
        public static void TryAppendUniqueTrackedObjects<TTracked, TDto>(this IList<TTracked> target,
            IEnumerable<TDto> dtos, ICacheRepository<TTracked> repository)
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

            //StreamTODO: change this to obtaining a hashset from a pool so (1) concurrent executions don't interfere (2) we use Hashset<TTracked> so there's no boxing
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
                    try
                    {
                        target.Add(trackedItem);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static readonly HashSet<object> _uniqueElements = new HashSet<object>();
    }
}