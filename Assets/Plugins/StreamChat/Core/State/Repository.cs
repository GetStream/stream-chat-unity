using System;
using System.Collections.Generic;
using StreamChat.Core.State.Models;
using StreamChat.Libs.Utils;

namespace StreamChat.Core.State
{
    internal class Repository<TTrackedType> : IRepository<TTrackedType>
        where TTrackedType : IStreamTrackedObject
    {
        public bool TryGet(string uniqueId, out TTrackedType trackedObject)
            => _trackedObjectById.TryGetValue(uniqueId, out trackedObject);

        public TTrackedType GetOrCreate(string uniqueId)
        {
            if (!TryGet(uniqueId, out var trackedObject))
            {
                trackedObject = _constructor(uniqueId, repository: this);
            }

            return trackedObject;
        }

        public void Track(TTrackedType trackedObject)
        {
            if (trackedObject.UniqueId.IsNullOrEmpty())
            {
                throw new ArgumentException($"{trackedObject.UniqueId} cannot be empty");
            }

            if (_channelsByCid.ContainsKey(trackedObject.UniqueId))
            {
                return;
            }

            _trackedObjectById[trackedObject.UniqueId] = trackedObject;
            _trackedObjects.Add(trackedObject);
        }

        internal delegate TTrackedType ConstructorHandler(string uniqueId, IRepository<TTrackedType> repository);

        internal Repository(ConstructorHandler constructor)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        private readonly List<StreamChannel> _channels = new List<StreamChannel>();
        private readonly Dictionary<string, StreamChannel> _channelsByCid = new Dictionary<string, StreamChannel>();

        private readonly List<TTrackedType> _trackedObjects = new List<TTrackedType>();
        private readonly Dictionary<string, TTrackedType> _trackedObjectById = new Dictionary<string, TTrackedType>();

        private readonly ConstructorHandler _constructor;
    }
}