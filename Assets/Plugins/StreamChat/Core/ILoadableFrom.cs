namespace StreamChat.Core
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
}