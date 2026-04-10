using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// Extensions for <see cref="ISavableTo{TDto}"/>
    /// </summary>
    internal static class ISavableToExt
    {
        public static TDto TrySaveToDto<TDto>(this ISavableTo<TDto> source)
            => source != default ? source.SaveToDto() : default;

        public static TDto TrySaveToDto<TDto>(this ISavableTo2<TDto> source)
            => source != default ? source.SaveToDto() : default;

        public static TDto TrySaveToDto<TDto>(this ISavableTo3<TDto> source)
            => source != default ? source.SaveToDto() : default;
    }
}