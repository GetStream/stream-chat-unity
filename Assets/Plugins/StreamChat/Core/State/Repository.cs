using System;
using System.Collections.Generic;
using StreamChat.Libs.Utils;

namespace StreamChat.Core.State
{
    //StreamTodo: CacheRepository?
    /// <summary>
    /// Tracked objects repository
    /// </summary>
    /// <typeparam name="TTrackedType">Tracked object type</typeparam>
    internal class Repository<TTrackedType> : IRepository<TTrackedType>
        where TTrackedType : class, IStreamTrackedObject
    {
        public IEnumerable<TTrackedType> AllItems => _trackedObjects;

        public bool TryGet(string uniqueId, out TTrackedType trackedObject)
            => _trackedObjectById.TryGetValue(uniqueId, out trackedObject);

        public string GetDtoTrackingId<TDto>(TDto dto)
        {
            var key = typeof(TDto);

            if (!_dtoIdGetters.ContainsKey(key))
            {
                throw new InvalidOperationException("Failed to find id getter for: " + key);
            }

            return _dtoIdGetters[key](dto);
        }

        /// <summary>
        /// Tracking ID will be used to retrieve cached object to which this DTO is mapped
        /// </summary>
        public void RegisterDtoTrackingIdGetter<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TTrackedType, IStreamTrackedObject, IUpdateableFrom<TDto, TType>
            where TDto : class
        {
            var key = typeof(TDto);

            if (_dtoIdGetters.ContainsKey(key))
            {
                throw new InvalidOperationException("Key is already registered: " + key);
            }

            string Wrapper(object obj) => idGetter(obj as TDto);

            _dtoIdGetters.Add(key, Wrapper);
        }

        public TType CreateOrUpdate<TType, TDto>(string uniqueId, TDto tdo)
            where TType : class, TTrackedType, IStreamTrackedObject, IUpdateableFrom<TDto, TType>
        {
            if (!TryGet(uniqueId, out var trackedObject))
            {
                trackedObject = _constructor(uniqueId, repository: this);
            }

            var typedTrackedObject = trackedObject as TType;
            if (typedTrackedObject == null)
            {
                throw new InvalidOperationException($"Failed to cast {typeof(TTrackedType)} to {typeof(TType)}");
            }

            typedTrackedObject.UpdateFromDto(tdo, _cache);

            return typedTrackedObject;
        }

        public TType CreateOrUpdate<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TTrackedType, IStreamTrackedObject, IUpdateableFrom<TDto, TType>
        {
            wasCreated = false;
            var trackingId = GetDtoTrackingId(dto);
            if (!TryGet(trackingId, out var trackedObject))
            {
                trackedObject = _constructor(trackingId, repository: this);
                wasCreated = true;
            }

            var typedTrackedObject = trackedObject as TType;
            if (typedTrackedObject == null)
            {
                throw new InvalidOperationException($"Failed to cast {typeof(TTrackedType)} to {typeof(TType)}");
            }

            typedTrackedObject.UpdateFromDto(dto, _cache);

            return typedTrackedObject;
        }

        /// <summary>
        /// This is called from tracked object constructor
        /// </summary>
        public void Track(TTrackedType trackedObject)
        {
            if (trackedObject.UniqueId.IsNullOrEmpty())
            {
                throw new ArgumentException($"{trackedObject.UniqueId} cannot be empty");
            }

            if (_trackedObjectById.ContainsKey(trackedObject.UniqueId))
            {
                throw new InvalidOperationException($"Object of type `{typeof(TTrackedType)}` and id {trackedObject.UniqueId} is already tracked");
            }

            _trackedObjectById[trackedObject.UniqueId] = trackedObject;
            _trackedObjects.Add(trackedObject);
        }

        public void Remove(TTrackedType trackedObject)
        {
            if (trackedObject.UniqueId.IsNullOrEmpty())
            {
                throw new ArgumentException($"{trackedObject.UniqueId} cannot be empty");
            }

            //StreamTodo: we could notify object that its being removed, perhaps IDIsposable?
            //This way object can release some memory before object is GCed

            _trackedObjects.Remove(trackedObject);
            _trackedObjectById.Remove(trackedObject.UniqueId);
        }

        internal delegate TTrackedType ConstructorHandler(string uniqueId, IRepository<TTrackedType> repository);

        internal Repository(ConstructorHandler constructor, ICache cache)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        private readonly List<TTrackedType> _trackedObjects = new List<TTrackedType>();
        private readonly Dictionary<string, TTrackedType> _trackedObjectById = new Dictionary<string, TTrackedType>();

        private readonly Dictionary<Type, Func<object, string>> _dtoIdGetters = new Dictionary<Type, Func<object, string>>();

        private readonly ConstructorHandler _constructor;
        private readonly ICache _cache;
        private readonly ITrackedObjectsFactory _trackedObjectsFactory;
    }
}