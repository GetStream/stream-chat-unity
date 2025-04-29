using System;
using System.Collections;
using System.Collections.Generic;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// Filtered list where items can only be added if they pass the filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class FilteredList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        public int Count => _internalList.Count;
        public bool IsReadOnly => false;

        public FilteredList(Predicate<T> addFilter)
        {
            _addFilter = addFilter ?? throw new ArgumentNullException(nameof(addFilter));
        }

        public IEnumerator<T> GetEnumerator() => _internalList.GetEnumerator();

        public void Add(T item)
        {
            if (_addFilter(item))
            {
                _internalList.Add(item);
            }
            else
            {
                throw new InvalidOperationException("Item does not pass the filter.");
            }
        }

        public void Clear() => _internalList.Clear();

        public bool Contains(T item) => _internalList.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _internalList.CopyTo(array, arrayIndex);

        public bool Remove(T item) => _internalList.Remove(item);

        public int IndexOf(T item) => _internalList.IndexOf(item);

        public void Insert(int index, T item)
        {
            if (_addFilter(item))
            {
                _internalList.Insert(index, item);
            }
            else
            {
                throw new InvalidOperationException("Item does not pass the filter.");
            }
        }

        public void RemoveAt(int index) => _internalList.RemoveAt(index);

        public T this[int index]
        {
            get => _internalList[index];
            set
            {
                if (!_addFilter(value))
                {
                    throw new InvalidOperationException("Item does not pass the filter.");
                }
                _internalList[index] = value;
            }
        }

        public void Sort(IComparer<T> comparer) => _internalList.Sort(comparer);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private readonly Predicate<T> _addFilter;
        private readonly List<T> _internalList = new List<T>();
    }
}