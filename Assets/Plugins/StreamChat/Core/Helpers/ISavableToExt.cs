using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// Extensions for <see cref="ISavableTo{TDto}"/>
    /// </summary>
    internal static class ISavableToExt
    {
        public static TDto TrySaveToDto<TDto>(this ISavableTo<TDto> source)
            where TDto : class
            => source?.SaveToDto();
    }
}