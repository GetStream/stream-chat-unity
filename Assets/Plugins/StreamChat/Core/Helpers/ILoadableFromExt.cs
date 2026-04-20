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

        public static TDomain TryLoadFromDto<TDto, TDomain>(this ILoadableFrom2<TDto, TDomain> loadable, TDto dto)
            where TDomain : ILoadableFrom2<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return default;
            }

            return loadable != null ? loadable.LoadFromDto(dto) : new TDomain().LoadFromDto(dto);
        }

        public static TDomain UpdateFromDto<TDto, TDomain>(this ILoadableFrom2<TDto, TDomain> loadable, TDto dto)
            where TDomain : ILoadableFrom2<TDto, TDomain>, new()
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

        public static TDomain ToDomain2<TDto, TDomain>(this TDto dto)
            where TDomain : class, ILoadableFrom2<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return null;
            }

            return new TDomain().LoadFromDto(dto);
        }

        public static TDomain TryLoadFromDto<TDto, TDomain>(this ILoadableFrom3<TDto, TDomain> loadable, TDto dto)
            where TDomain : ILoadableFrom3<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return default;
            }

            return loadable != null ? loadable.LoadFromDto(dto) : new TDomain().LoadFromDto(dto);
        }

        public static TDomain UpdateFromDto<TDto, TDomain>(this ILoadableFrom3<TDto, TDomain> loadable, TDto dto)
            where TDomain : ILoadableFrom3<TDto, TDomain>, new()
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

        public static TDomain ToDomain3<TDto, TDomain>(this TDto dto)
            where TDomain : class, ILoadableFrom3<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return null;
            }

            return new TDomain().LoadFromDto(dto);
        }

        public static TDomain TryLoadFromDto<TDto, TDomain>(this ILoadableFrom4<TDto, TDomain> loadable, TDto dto)
            where TDomain : ILoadableFrom4<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return default;
            }

            return loadable != null ? loadable.LoadFromDto(dto) : new TDomain().LoadFromDto(dto);
        }

        public static TDomain UpdateFromDto<TDto, TDomain>(this ILoadableFrom4<TDto, TDomain> loadable, TDto dto)
            where TDomain : ILoadableFrom4<TDto, TDomain>, new()
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

        public static TDomain ToDomain4<TDto, TDomain>(this TDto dto)
            where TDomain : class, ILoadableFrom4<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return null;
            }

            return new TDomain().LoadFromDto(dto);
        }
    }
}