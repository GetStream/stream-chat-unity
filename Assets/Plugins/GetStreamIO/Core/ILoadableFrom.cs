namespace Plugins.GetStreamIO.Core
{
    /// <summary>
    /// Supports loading object from DTO of a given type
    /// </summary>
    /// <typeparam name="TDto">DTO type</typeparam>
    /// <typeparam name="TDomain">Domain object type</typeparam>
    public interface ILoadableFrom<in TDto, out TDomain>
        where TDomain : ILoadableFrom<TDto, TDomain>
    {
        TDomain LoadFromDto(TDto dto);
    }

    /// <summary>
    /// Extensions for <see cref="ILoadableFrom"/>
    /// </summary>
    public static class ILoadableFromExt
    {
        public static TDomain TryLoadFromDto<TDto, TDomain>(this ILoadableFrom<TDto, TDomain> loadable, TDto dto)
            where TDomain : class, ILoadableFrom<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return null;
            }

            return new TDomain().LoadFromDto(dto);
        }
    }
}