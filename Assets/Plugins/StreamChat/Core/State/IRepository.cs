using System;

namespace StreamChat.Core.State
{
    /// <summary>
    /// Repository for tracked objects
    /// </summary>
    /// <typeparam name="TTrackedObject">Tracked object type</typeparam>
    internal interface IRepository<TTrackedObject>
        where TTrackedObject : class, IStreamTrackedObject
    {
        bool TryGet(string uniqueId, out TTrackedObject trackedObject);

        //TTrackedObject GetOrCreate(string uniqueId);

        TTrackedType CreateOrUpdate<TTrackedType, TDto>(string uniqueId, TDto tdo)
            where TTrackedType : class, TTrackedObject, IStreamTrackedObject, IUpdateableFrom<TDto, TTrackedType>;

        void Track(TTrackedObject trackedObject);

        string GetDtoTrackingId<TDto>(TDto dto);

        void RegisterDtoTrackingIdGetter<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TTrackedObject, IStreamTrackedObject, IUpdateableFrom<TDto, TType>
            where TDto : class;

        TType CreateOrUpdate<TType, TDto>(TDto dto)
            where TType : class, TTrackedObject, IStreamTrackedObject, IUpdateableFrom<TDto, TType>;
    }
}