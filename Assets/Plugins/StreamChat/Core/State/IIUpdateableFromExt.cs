using StreamChat.Core.State.Caches;

namespace StreamChat.Core.State
{
    internal static class IIUpdateableFromExt
    {
        public static void TryUpdateFromDto<TDto, TTrackedObject>(this IUpdateableFrom<TDto, TTrackedObject> updateable, TDto dto, ICache cache)
            where TTrackedObject : IStreamTrackedObject, IUpdateableFrom<TDto, TTrackedObject>
        {
            updateable?.UpdateFromDto(dto, cache);
        }
    }
}