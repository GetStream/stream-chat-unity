﻿namespace Plugins.GetStreamIO.Core.Helpers
{
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