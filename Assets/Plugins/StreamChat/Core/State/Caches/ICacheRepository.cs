using System;
using System.Collections.Generic;

namespace StreamChat.Core.State.Caches
{
    /// <summary>
    /// Repository for tracked objects
    /// </summary>
    /// <typeparam name="TTrackedObject">Tracked object type</typeparam>
    internal interface ICacheRepository<TTrackedObject>
        where TTrackedObject : class, IStreamTrackedObject
    {
        IReadOnlyList<TTrackedObject> AllItems { get; }

        bool TryGet(string uniqueId, out TTrackedObject trackedObject);
        
        void Track(TTrackedObject trackedObject);
        
        void RegisterDtoTrackingIdGetter<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TTrackedObject, IStreamTrackedObject, IUpdateableFrom<TDto, TType>
            where TDto : class;

        TType CreateOrUpdate<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TTrackedObject, IStreamTrackedObject, IUpdateableFrom<TDto, TType>;

        void Remove(TTrackedObject trackedObject);
    }
}