using StreamChat.Core.State;

namespace StreamChat.Core.StreamChat.Core.State
{
    /// <summary>
    /// Base class for tracked objects. Read more: <see cref="IStreamTrackedObject"/>
    /// </summary>
    /// <typeparam name="TTrackedObject">Type of tracked object</typeparam>
    public abstract class StreamTrackedObjectBase<TTrackedObject> : IStreamTrackedObject
        where TTrackedObject : IStreamTrackedObject
    {
        string IStreamTrackedObject.UniqueId => InternalUniqueId;

        internal StreamTrackedObjectBase(string uniqueId, IRepository<TTrackedObject> repository)
        {
            InternalUniqueId = uniqueId;
            repository.Track(Self);
        }

        protected abstract string InternalUniqueId { get; set; }

        protected abstract TTrackedObject Self { get; }
    }
}