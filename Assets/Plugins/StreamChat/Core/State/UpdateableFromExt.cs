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

        public static void TryUpdateFromDto<TDto, TTrackedObject>(this IUpdateableFrom2<TDto, TTrackedObject> updateable, TDto dto, ICache cache)
            where TTrackedObject : IStreamStatefulModel, IUpdateableFrom2<TDto, TTrackedObject>
        {
            updateable?.UpdateFromDto(dto, cache);
        }

        public static void TryUpdateFromDto<TDto, TTrackedObject>(this IUpdateableFrom3<TDto, TTrackedObject> updateable, TDto dto, ICache cache)
            where TTrackedObject : IStreamStatefulModel, IUpdateableFrom3<TDto, TTrackedObject>
        {
            updateable?.UpdateFromDto(dto, cache);
        }

        public static void TryUpdateFromDto<TDto, TTrackedObject>(this IUpdateableFrom4<TDto, TTrackedObject> updateable, TDto dto, ICache cache)
            where TTrackedObject : IStreamStatefulModel, IUpdateableFrom4<TDto, TTrackedObject>
        {
            updateable?.UpdateFromDto(dto, cache);
        }

        public static void TryUpdateFromDto<TDto, TTrackedObject>(this IUpdateableFrom5<TDto, TTrackedObject> updateable, TDto dto, ICache cache)
            where TTrackedObject : IStreamStatefulModel, IUpdateableFrom5<TDto, TTrackedObject>
        {
            updateable?.UpdateFromDto(dto, cache);
        }
    }
}