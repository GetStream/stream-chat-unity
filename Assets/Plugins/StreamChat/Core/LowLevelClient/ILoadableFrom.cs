namespace StreamChat.Core.LowLevelClient
{
    /// <summary>
    /// Supports loading object from DTO of a given type
    /// </summary>
    /// <typeparam name="TDto">DTO type</typeparam>
    /// <typeparam name="TDomain">Domain object type</typeparam>
    internal interface ILoadableFrom<in TDto, out TDomain>
        where TDomain : ILoadableFrom<TDto, TDomain>
    {
        TDomain LoadFromDto(TDto dto);
    }

    internal interface ILoadableFrom2<in TDto, out TDomain>
        where TDomain : ILoadableFrom2<TDto, TDomain>
    {
        TDomain LoadFromDto(TDto dto);
    }

    internal interface ILoadableFrom3<in TDto, out TDomain>
        where TDomain : ILoadableFrom3<TDto, TDomain>
    {
        TDomain LoadFromDto(TDto dto);
    }

    internal interface ILoadableFrom4<in TDto, out TDomain>
        where TDomain : ILoadableFrom4<TDto, TDomain>
    {
        TDomain LoadFromDto(TDto dto);
    }
}