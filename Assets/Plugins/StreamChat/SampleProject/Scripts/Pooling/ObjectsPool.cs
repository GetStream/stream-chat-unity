using System;
using System.Collections.Generic;
using UnityEngine;

namespace StreamChat.SampleProject.Pooling
{
    public class ObjectsPool<TItem> where TItem : IPoolItem
    {
        public ObjectsPool(Func<TItem> create, Action<TItem> destroy)
        {
            _create = create ?? throw new ArgumentNullException(nameof(create));
            _destroy = destroy ?? throw new ArgumentNullException(nameof(destroy));
        }

        public void Prewarm(uint count)
        {
            for (int i = 0; i < count; i++)
            {
                var item = CreateNewItem();
                _availableItems.Enqueue(item);
            }
        }

        public TItem Rent()
        {
            TItem item;
            if (_availableItems.Count > 0)
            {
                item = _availableItems.Dequeue();
            }
            else
            {
                item = CreateNewItem();
            }
            
            item.OnRenting();

            return item;
        }

        public void Return(TItem item)
        {
            item.OnReturning();
            _availableItems.Enqueue(item);
        }

        public void DestroyAllItems()
        {
            for (var i = _allItems.Count - 1; i >= 0; i--)
            {
                _destroy(_allItems[i]);
            }

            _allItems.Clear();
        }

        private readonly List<TItem> _allItems = new List<TItem>();
        private readonly Queue<TItem> _availableItems = new Queue<TItem>();

        private readonly Func<TItem> _create;
        private readonly Action<TItem> _destroy;
        
        private TItem CreateNewItem()
        {
            Debug.Log("Pool Item Created. Total Count: " + _allItems.Count);
            var item = _create();
            _allItems.Add(item);
            return item;
        }
    }
}