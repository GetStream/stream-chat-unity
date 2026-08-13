using System;
using System.Collections.Generic;

namespace StreamChat.Core.Helpers
{
    internal static class HashSetPool<T>
    {
        public static HashSet<T> Rent()
        {
            if (Pool.Count > 0)
            {
                return Pool.Pop();
            }

            return new HashSet<T>();
        }

        public static void Release(HashSet<T> set)
        {
            if (set == null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            // HashSet has no Capacity accessor, so the pre-clear count stands in for how large the
            // buckets grew. See the same guard in ListPool.
            var isOversized = set.Count > MaxRetainedCount;

            set.Clear();

            if (!isOversized && Pool.Count < MaxPoolSize)
            {
                Pool.Push(set);
            }
        }

        private const int MaxPoolSize = 128;
        private const int MaxRetainedCount = 4096;
        private static readonly Stack<HashSet<T>> Pool = new Stack<HashSet<T>>();
    }
}
