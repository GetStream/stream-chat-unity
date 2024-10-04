using System;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// Extensions for <see cref="IEnumeratedStruct{TType}"/>
    /// </summary>
    internal static class EnumeratedStructExt
    {
        public static TDomain? TryLoadNullableStructFromDto<TDomain, TDto>(this TDomain? domain, TDto? dto)
            where TDomain : struct, IEquatable<TDomain>, ILoadableFrom<TDto, TDomain>
            where TDto : struct, IEnumeratedStruct<TDto>
        {
            if (!dto.HasValue)
            {
                return null;
            }

            return domain?.LoadFromDto(dto.Value) ?? default(TDomain).LoadFromDto(dto.Value);
        }
        
        public static TDTO? TrySaveNullableStructToDto<TDomain, TDTO>(this TDomain? domain)
            where TDomain : struct, IEquatable<TDomain>, ISavableTo<TDTO>
            where TDTO : struct, IEnumeratedStruct<TDTO>
        {
            return domain?.SaveToDto();
        }
    }
}