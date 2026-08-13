using System;
using System.Collections.Generic;
using StreamChat.Core.Helpers;
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
        public event Action<TStatefulModel> Tracked;
        public event Action<TStatefulModel> Untracked;

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
            RegisterDtoIdMappingInternal(idGetter);
        }

        public void RegisterDtoIdMapping2<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TStatefulModel, IStreamStatefulModel, IUpdateableFrom2<TDto, TType>
            where TDto : class
        {
            RegisterDtoIdMappingInternal(idGetter);
        }

        public void RegisterDtoIdMapping3<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TStatefulModel, IStreamStatefulModel, IUpdateableFrom3<TDto, TType>
            where TDto : class
        {
            RegisterDtoIdMappingInternal(idGetter);
        }

        public void RegisterDtoIdMapping4<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TStatefulModel, IStreamStatefulModel, IUpdateableFrom4<TDto, TType>
            where TDto : class
        {
            RegisterDtoIdMappingInternal(idGetter);
        }

        public void RegisterDtoIdMapping5<TType, TDto>(Func<TDto, string> idGetter)
            where TType : class, TStatefulModel, IStreamStatefulModel, IUpdateableFrom5<TDto, TType>
            where TDto : class
        {
            RegisterDtoIdMappingInternal(idGetter);
        }

        public TType CreateOrUpdate<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TStatefulModel, IStreamStatefulModel, IUpdateableFrom<TDto, TType>
        {
            var typedStatefulModel = GetOrCreateStatefulModel<TType, TDto>(dto, out wasCreated);
            typedStatefulModel.UpdateFromDto(dto, _cache);
            RaiseTrackedIfCreated(typedStatefulModel, wasCreated);
            return typedStatefulModel;
        }

        public TType CreateOrUpdate2<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TStatefulModel, IStreamStatefulModel, IUpdateableFrom2<TDto, TType>
        {
            var typedStatefulModel = GetOrCreateStatefulModel<TType, TDto>(dto, out wasCreated);
            typedStatefulModel.UpdateFromDto(dto, _cache);
            RaiseTrackedIfCreated(typedStatefulModel, wasCreated);
            return typedStatefulModel;
        }

        public TType CreateOrUpdate3<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TStatefulModel, IStreamStatefulModel, IUpdateableFrom3<TDto, TType>
        {
            var typedStatefulModel = GetOrCreateStatefulModel<TType, TDto>(dto, out wasCreated);
            typedStatefulModel.UpdateFromDto(dto, _cache);
            RaiseTrackedIfCreated(typedStatefulModel, wasCreated);
            return typedStatefulModel;
        }

        public TType CreateOrUpdate4<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TStatefulModel, IStreamStatefulModel, IUpdateableFrom4<TDto, TType>
        {
            var typedStatefulModel = GetOrCreateStatefulModel<TType, TDto>(dto, out wasCreated);
            typedStatefulModel.UpdateFromDto(dto, _cache);
            RaiseTrackedIfCreated(typedStatefulModel, wasCreated);
            return typedStatefulModel;
        }

        public TType CreateOrUpdate5<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TStatefulModel, IStreamStatefulModel, IUpdateableFrom5<TDto, TType>
        {
            var typedStatefulModel = GetOrCreateStatefulModel<TType, TDto>(dto, out wasCreated);
            typedStatefulModel.UpdateFromDto(dto, _cache);
            RaiseTrackedIfCreated(typedStatefulModel, wasCreated);
            return typedStatefulModel;
        }

        // Defer Tracked emission until AFTER the first UpdateFromDto so subscribers always observe
        // a fully-hydrated object. Track() itself runs from the StreamStatefulModelBase constructor,
        // before any DTO is applied, so emitting from there would surface a blank instance.
        private void RaiseTrackedIfCreated(TStatefulModel trackedObject, bool wasCreated)
        {
            if (wasCreated)
            {
                Tracked?.Invoke(trackedObject);
            }
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

            Untracked?.Invoke(trackedObject);
        }

        public void RemoveMany(IReadOnlyList<TStatefulModel> trackedObjects)
        {
            if (trackedObjects == null)
            {
                throw new ArgumentNullException(nameof(trackedObjects));
            }

            if (trackedObjects.Count == 0)
            {
                return;
            }

            using (new HashSetPoolScope<string>(out var tempRemovedIds))
            using (new ListPoolScope<TStatefulModel>(out var tempRemoved))
            {
                for (var i = 0; i < trackedObjects.Count; i++)
                {
                    var trackedObject = trackedObjects[i];
                    if (trackedObject.UniqueId.IsNullOrEmpty())
                    {
                        throw new ArgumentException($"{trackedObject.UniqueId} cannot be empty");
                    }

                    // Only untrack the exact instance this repository holds. A newer instance for the
                    // same id must survive, and duplicates in the input must not raise Untracked twice.
                    if (!_statefulModelById.TryGetValue(trackedObject.UniqueId, out var tracked)
                        || !ReferenceEquals(tracked, trackedObject))
                    {
                        continue;
                    }

                    _statefulModelById.Remove(trackedObject.UniqueId);
                    tempRemovedIds.Add(trackedObject.UniqueId);
                    tempRemoved.Add(trackedObject);
                }

                if (tempRemoved.Count == 0)
                {
                    return;
                }

                _statefulModels.RemoveAll(_ => tempRemovedIds.Contains(_.UniqueId));

                for (var i = 0; i < tempRemoved.Count; i++)
                {
                    Untracked?.Invoke(tempRemoved[i]);
                }
            }
        }

        internal delegate TStatefulModel ConstructorHandler(string uniqueId);

        internal CacheRepository(ConstructorHandler constructor, ICache cache)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        private void RegisterDtoIdMappingInternal<TDto>(Func<TDto, string> idGetter) where TDto : class
        {
            var key = typeof(TDto);

            if (_dtoIdGetters.ContainsKey(key))
            {
                throw new InvalidOperationException("Key is already registered: " + key);
            }

            string Wrapper(object obj) => idGetter(obj as TDto);

            _dtoIdGetters.Add(key, Wrapper);
        }

        private TType GetOrCreateStatefulModel<TType, TDto>(TDto dto, out bool wasCreated)
            where TType : class, TStatefulModel, IStreamStatefulModel
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

            return typedStatefulModel;
        }

        private readonly List<TStatefulModel> _statefulModels = new List<TStatefulModel>();
        private readonly Dictionary<string, TStatefulModel> _statefulModelById = new Dictionary<string, TStatefulModel>();

        private readonly Dictionary<Type, Func<object, string>> _dtoIdGetters = new Dictionary<Type, Func<object, string>>();

        private readonly ConstructorHandler _constructor;
        private readonly ICache _cache;
    }
}