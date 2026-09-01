using System;
using System.Collections.Generic;

namespace StreamChat.Core.Helpers
{
    internal readonly struct ListPoolScope<T> : IDisposable
    {
        public ListPoolScope(out List<T> list)
        {
            _list = ListPool<T>.Rent();
            list = _list;
        }

        public void Dispose()
        {
            if (_list != null)
            {
                ListPool<T>.Release(_list);
            }
        }

        private readonly List<T> _list;
    }
}
