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
        /// <summary>
        /// Raised after a tracked object is first hydrated by a CreateOrUpdate call (the first
        /// time a given DTO id is observed). Fires after the initial UpdateFromDto completes,
        /// so subscribers always observe a populated object - never a blank instance.
        /// </summary>
        event Action<TTrackedObject> Tracked;

        /// <summary>
        /// Raised after a tracked object is removed from the repository.
        /// </summary>
        event Action<TTrackedObject> Untracked;

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

        /// <summary>
        /// Removes multiple tracked objects in a single pass. Prefer this over calling
        /// <see cref="Remove"/> in a loop - removing N objects one by one is O(N * repository size).
        /// </summary>
        void RemoveMany(IReadOnlyList<TTrackedObject> trackedObjects);
    }
}