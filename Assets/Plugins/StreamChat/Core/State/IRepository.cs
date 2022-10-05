namespace StreamChat.Core.State
{
    /// <summary>
    /// Repository for tracked objects
    /// </summary>
    /// <typeparam name="TTrackedObject">Tracked object type</typeparam>
    internal interface IRepository<TTrackedObject>
        where TTrackedObject : IStreamTrackedObject
    {
        bool TryGet(string uniqueId, out TTrackedObject trackedObject);

        TTrackedObject GetOrCreate(string uniqueId);

        void Track(TTrackedObject trackedObject);
    }
}