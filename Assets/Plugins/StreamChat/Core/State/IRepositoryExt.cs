﻿using System;
using System.Collections.Generic;

namespace StreamChat.Core.State
{
    internal static class IRepositoryExt
    {
        /// <summary>
        /// Clear target list and replace with items created or updated from DTO collection
        /// </summary>
        public static void TryReplaceTrackedObjects<TTracked, TDto>(this IList<TTracked> target, IEnumerable<TDto> dtos,
            IRepository<TTracked> repository)
            where TTracked : class, IStreamTrackedObject, IUpdateableFrom<TDto, TTracked>
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
    }
}