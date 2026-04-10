using StreamChat.Core.State.Caches;

namespace StreamChat.Core.State
{
    internal interface IUpdateableFrom<in TDto, out TTrackedObject>
        where TTrackedObject : IStreamStatefulModel, IUpdateableFrom<TDto, TTrackedObject>
    {
        void UpdateFromDto(TDto dto, ICache cache);
    }

    internal interface IUpdateableFrom2<in TDto, out TTrackedObject>
        where TTrackedObject : IStreamStatefulModel, IUpdateableFrom2<TDto, TTrackedObject>
    {
        void UpdateFromDto(TDto dto, ICache cache);
    }

    internal interface IUpdateableFrom3<in TDto, out TTrackedObject>
        where TTrackedObject : IStreamStatefulModel, IUpdateableFrom3<TDto, TTrackedObject>
    {
        void UpdateFromDto(TDto dto, ICache cache);
    }

    internal interface IUpdateableFrom4<in TDto, out TTrackedObject>
        where TTrackedObject : IStreamStatefulModel, IUpdateableFrom4<TDto, TTrackedObject>
    {
        void UpdateFromDto(TDto dto, ICache cache);
    }

    internal interface IUpdateableFrom5<in TDto, out TTrackedObject>
        where TTrackedObject : IStreamStatefulModel, IUpdateableFrom5<TDto, TTrackedObject>
    {
        void UpdateFromDto(TDto dto, ICache cache);
    }
}