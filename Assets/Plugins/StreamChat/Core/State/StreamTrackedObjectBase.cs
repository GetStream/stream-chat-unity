using System.Collections.Generic;

namespace StreamChat.Core.State
{
    /// <summary>
    /// Base class for tracked objects. Read more: <see cref="IStreamTrackedObject"/>
    /// </summary>
    /// <typeparam name="TTrackedObject">Type of tracked object</typeparam>
    public abstract class StreamTrackedObjectBase<TTrackedObject> : IStreamTrackedObject
        where TTrackedObject : class, IStreamTrackedObject
    {
        string IStreamTrackedObject.UniqueId => InternalUniqueId;

        public void SetCustomField(string key, object value) => _additionalProperties[key] = value;

        public bool TryGetCustomField(string key, out object value)
            => _additionalProperties.TryGetValue(key, out value);

        internal StreamTrackedObjectBase(string uniqueId, IRepository<TTrackedObject> repository)
        {
            InternalUniqueId = uniqueId;
            repository.Track(Self);
        }

        protected abstract string InternalUniqueId { get; set; }

        protected abstract TTrackedObject Self { get; }

        protected void LoadAdditionalProperties(Dictionary<string, object> additionalProperties)
        {
            _additionalProperties.Clear();

            foreach (var keyValuePair in additionalProperties)
            {
                _additionalProperties.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        private readonly Dictionary<string, object> _additionalProperties = new Dictionary<string, object>();

    }
}