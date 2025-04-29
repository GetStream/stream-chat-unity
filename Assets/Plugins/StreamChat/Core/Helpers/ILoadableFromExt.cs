using System;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// Extensions for <see cref="ILoadableFrom{TDto,TDomain}"/>
    /// </summary>
    internal static class ILoadableFromExt
    {
        //StreamTOdo: rename to TryCreateOrLoadFromDto
        /// <summary>
        /// Load domain object from the DTO. If the loadable is null, creates a new instance of the domain object.
        /// </summary>
        public static TDomain TryLoadFromDto<TDto, TDomain>(this ILoadableFrom<TDto, TDomain> loadable, TDto dto)
            where TDomain : ILoadableFrom<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return default;
            }

            return loadable != null ? loadable.LoadFromDto(dto) : new TDomain().LoadFromDto(dto);
        }
        
        public static TDomain UpdateFromDto<TDto, TDomain>(this ILoadableFrom<TDto, TDomain> loadable, TDto dto)
            where TDomain : ILoadableFrom<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return default;
            }

            if (loadable == null)
            {
                throw new ArgumentException(nameof(loadable));
            }

            return loadable.LoadFromDto(dto);
        }

        public static TDomain ToDomain<TDto, TDomain>(this TDto dto)
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