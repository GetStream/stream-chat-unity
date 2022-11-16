using System;
using System.Collections.Generic;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.State.Caches;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.State
{
    /// <summary>
    /// Base class for tracked objects. Read more: <see cref="IStreamTrackedObject"/>
    /// </summary>
    /// <typeparam name="TTrackedObject">Type of tracked object</typeparam>
    internal abstract class StreamTrackedObjectBase<TTrackedObject> : IStreamTrackedObject
        where TTrackedObject : class, IStreamTrackedObject
    {
        public IDictionary<string, object> CustomData => _additionalProperties;
        
        string IStreamTrackedObject.UniqueId => InternalUniqueId;

        public object SetCustomData(string key, object value) => _additionalProperties[key] = value;
        public object GetCustomData(string key) => _additionalProperties[key];

        internal StreamTrackedObjectBase(string uniqueId, ICacheRepository<TTrackedObject> repository,
            ITrackedObjectContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Client = context.Client ?? throw new ArgumentNullException(nameof(context.Client));
            Logs = context.Logs ?? throw new ArgumentNullException(nameof(context.Logs));
            Cache = context.Cache ?? throw new ArgumentNullException(nameof(context.Cache));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));

            InternalUniqueId = uniqueId;
            Repository.Track(Self);
        }

        protected abstract string InternalUniqueId { get; set; }

        protected abstract TTrackedObject Self { get; }
        protected StreamChatClient Client { get; }
        protected StreamChatLowLevelClient LowLevelClient => Client.LowLevelClient;
        protected ILogs Logs { get; }
        internal ICache Cache { get; }
        internal ICacheRepository<TTrackedObject> Repository { get; }

        protected void LoadAdditionalProperties(Dictionary<string, object> additionalProperties)
        {
            _additionalProperties.Clear();

            foreach (var keyValuePair in additionalProperties)
            {
                _additionalProperties.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        protected static bool TrySet<T>(ref T storage, T value)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            return true;
        }

        protected static T GetOrDefault<T>(T? source, T defaultValue)
            where T : struct
            => source ?? defaultValue;

        protected static T? GetOrDefault<T>(T? source, T? defaultValue)
            where T : struct
            => source ?? defaultValue;

        protected static T GetOrDefault<T>(T source, T defaultValue)
            where T : class
            => source ?? defaultValue;

        private readonly Dictionary<string, object> _additionalProperties = new Dictionary<string, object>();
    }
}