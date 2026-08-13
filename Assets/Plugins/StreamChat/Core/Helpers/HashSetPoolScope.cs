using System;
using System.Collections.Generic;

namespace StreamChat.Core.Helpers
{
    internal readonly struct HashSetPoolScope<T> : IDisposable
    {
        public HashSetPoolScope(out HashSet<T> set)
        {
            _set = HashSetPool<T>.Rent();
            set = _set;
        }

        public void Dispose()
        {
            if (_set != null)
            {
                HashSetPool<T>.Release(_set);
            }
        }

        private readonly HashSet<T> _set;
    }
}
