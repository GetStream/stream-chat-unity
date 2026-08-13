using System;
using System.Collections.Generic;

namespace StreamChat.Core.Helpers
{
    internal static class ListPool<T>
    {
        public static List<T> Rent()
        {
            if (Pool.Count > 0)
            {
                return Pool.Pop();
            }

            return new List<T>();
        }

        public static void Release(List<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            // Clear() keeps the backing array, so a one-off bulk operation would otherwise retain an
            // oversized buffer for the rest of the session. Dropping it costs one allocation later.
            var isOversized = list.Capacity > MaxRetainedCapacity;

            list.Clear();

            if (!isOversized && Pool.Count < MaxPoolSize)
            {
                Pool.Push(list);
            }
        }

        private const int MaxPoolSize = 128;
        private const int MaxRetainedCapacity = 4096;
        private static readonly Stack<List<T>> Pool = new Stack<List<T>>();
    }
}
