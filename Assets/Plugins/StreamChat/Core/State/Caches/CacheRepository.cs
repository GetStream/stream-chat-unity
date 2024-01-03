using System;
using System.Collections.Generic;
using StreamChat.Libs.Utils;

namespace StreamChat.Core.State.Caches
{
    /// <summary>
    /// Cache repository for <see cref="IStreamStatefulModel"/>
    /// </summary>
    /// <typeparam name="TStatefulModel">Tracked object type</typeparam>
    internal sealed class CacheRepository<TStatefulModel> : ICacheRepository<TStatefulModel>
        where TStatefulModel : class, IStreamStatefulModel
    {
        public IReadOnlyList<TStatefulModel> AllItems => _statefulModels;

        public bool TryGet(string uniqueId, out TStatefulModel trackedObject)
            => _statefulModelById.TryGetValue(uniqueId, out trackedObject);

        private string GetDtoMappingId<TDto>(TDto dto)
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
        public void RegisterDtoIdMapping<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TStatefulModel, IStreamStatefulModel, IUpdateableFrom<TDto, TType>
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

        public TType CreateOrUpdate<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TStatefulModel, IStreamStatefulModel, IUpdateableFrom<TDto, TType>
        {
            wasCreated = false;
            var trackingId = GetDtoMappingId(dto);
            if (!TryGet(trackingId, out var trackedObject))
            {
                trackedObject = _constructor(trackingId);
                wasCreated = true;
            }

            var typedStatefulModel = trackedObject as TType;
            if (typedStatefulModel == null)
            {
                throw new InvalidOperationException($"Failed to cast {typeof(TStatefulModel)} to {typeof(TType)}");
            }

            typedStatefulModel.UpdateFromDto(dto, _cache);

            return typedStatefulModel;
        }

        /// <summary>
        /// This is called from <see cref="IStreamStatefulModel"/> constructor
        /// </summary>
        public void Track(TStatefulModel trackedObject)
        {
            if (trackedObject.UniqueId.IsNullOrEmpty())
            {
                throw new ArgumentException($"{trackedObject.UniqueId} cannot be empty");
            }

            if (_statefulModelById.ContainsKey(trackedObject.UniqueId))
            {
                throw new InvalidOperationException($"Object of type `{typeof(TStatefulModel)}` and id {trackedObject.UniqueId} is already tracked");
            }

            _statefulModelById[trackedObject.UniqueId] = trackedObject;
            _statefulModels.Add(trackedObject);
        }

        public void Remove(TStatefulModel trackedObject)
        {
            if (trackedObject.UniqueId.IsNullOrEmpty())
            {
                throw new ArgumentException($"{trackedObject.UniqueId} cannot be empty");
            }

            //StreamTodo: we could notify object that its being removed, perhaps IDIsposable?
            //This way object can release some memory before object is GCed

            _statefulModels.Remove(trackedObject);
            _statefulModelById.Remove(trackedObject.UniqueId);
        }

        internal delegate TStatefulModel ConstructorHandler(string uniqueId);

        internal CacheRepository(ConstructorHandler constructor, ICache cache)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        private readonly List<TStatefulModel> _statefulModels = new List<TStatefulModel>();
        private readonly Dictionary<string, TStatefulModel> _statefulModelById = new Dictionary<string, TStatefulModel>();

        private readonly Dictionary<Type, Func<object, string>> _dtoIdGetters = new Dictionary<Type, Func<object, string>>();

        private readonly ConstructorHandler _constructor;
        private readonly ICache _cache;
    }
}