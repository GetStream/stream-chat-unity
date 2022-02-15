namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// Extensions for <see cref="ILoadableFrom{TDto,TDomain}"/>
    /// </summary>
    internal static class ILoadableFromExt
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