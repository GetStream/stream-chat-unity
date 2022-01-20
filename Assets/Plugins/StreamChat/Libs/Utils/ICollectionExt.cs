using System.Collections.Generic;

namespace StreamChat.Libs.Utils
{
    public static class ICollectionExt
    {
        public static ICollection<T> AddFluent<T>(this ICollection<T> collection, T item)
        {
            collection.Add(item);
            return collection;
        }
    }
}