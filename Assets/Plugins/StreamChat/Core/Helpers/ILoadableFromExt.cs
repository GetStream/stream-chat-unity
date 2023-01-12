using System;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// Extensions for <see cref="ILoadableFrom{TDto,TDomain}"/>
    /// </summary>
    internal static class ILoadableFromExt
    {
        //StreamTodo: rename to TryCreateFromDto? It's misleading because it creates new instance which you need to replace
        public static TDomain TryLoadFromDto<TDto, TDomain>(this ILoadableFrom<TDto, TDomain> loadable, TDto dto)
            where TDomain : class, ILoadableFrom<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return null;
            }

            return new TDomain().LoadFromDto(dto);
        }
        
        public static TDomain UpdateFromDto<TDto, TDomain>(this ILoadableFrom<TDto, TDomain> loadable, TDto dto)
            where TDomain : class, ILoadableFrom<TDto, TDomain>, new()
        {
            if (dto == null)
            {
                return null;
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