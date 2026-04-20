using System;
using System.Collections.Generic;

namespace StreamChat.Core.State.Caches
{
    /// <summary>
    /// Repository for tracked objects
    /// </summary>
    /// <typeparam name="TTrackedObject">Tracked object type</typeparam>
    internal interface ICacheRepository<TTrackedObject>
        where TTrackedObject : class, IStreamStatefulModel
    {
        IReadOnlyList<TTrackedObject> AllItems { get; }

        bool TryGet(string uniqueId, out TTrackedObject trackedObject);
        
        void Track(TTrackedObject trackedObject);
        
        void RegisterDtoIdMapping<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TTrackedObject, IStreamStatefulModel, IUpdateableFrom<TDto, TType>
            where TDto : class;

        void RegisterDtoIdMapping2<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TTrackedObject, IStreamStatefulModel, IUpdateableFrom2<TDto, TType>
            where TDto : class;

        void RegisterDtoIdMapping3<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TTrackedObject, IStreamStatefulModel, IUpdateableFrom3<TDto, TType>
            where TDto : class;

        void RegisterDtoIdMapping4<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TTrackedObject, IStreamStatefulModel, IUpdateableFrom4<TDto, TType>
            where TDto : class;

        void RegisterDtoIdMapping5<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TTrackedObject, IStreamStatefulModel, IUpdateableFrom5<TDto, TType>
            where TDto : class;

        TType CreateOrUpdate<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TTrackedObject, IStreamStatefulModel, IUpdateableFrom<TDto, TType>;

        TType CreateOrUpdate2<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TTrackedObject, IStreamStatefulModel, IUpdateableFrom2<TDto, TType>;

        TType CreateOrUpdate3<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TTrackedObject, IStreamStatefulModel, IUpdateableFrom3<TDto, TType>;

        TType CreateOrUpdate4<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TTrackedObject, IStreamStatefulModel, IUpdateableFrom4<TDto, TType>;

        TType CreateOrUpdate5<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TTrackedObject, IStreamStatefulModel, IUpdateableFrom5<TDto, TType>;

        void Remove(TTrackedObject trackedObject);
    }
}