namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// Extensions for <see cref="ISavableTo{TDto}"/>
    /// </summary>
    public static class ISavableToExt
    {
        public static TDto TrySaveToDto<TDto>(this ISavableTo<TDto> source)
            where TDto : class
            => source?.SaveToDto();
    }
}