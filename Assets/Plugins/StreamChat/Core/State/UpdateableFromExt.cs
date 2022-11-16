using StreamChat.Core.State.Caches;

namespace StreamChat.Core.State
{
    internal static class UpdateableFromExt
    {
        public static void TryUpdateFromDto<TDto, TTrackedObject>(this IUpdateableFrom<TDto, TTrackedObject> updateable, TDto dto, ICache cache)
            where TTrackedObject : IStreamStatefulModel, IUpdateableFrom<TDto, TTrackedObject>
        {
            updateable?.UpdateFromDto(dto, cache);
        }
    }
}