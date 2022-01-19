namespace Plugins.GetStreamIO.Core.Helpers
{
    /// <summary>
    /// Extensions for <see cref="ISavableTo"/>
    /// </summary>
    public static class ISavableToExt
    {
        public static TDto TrySaveToDto<TDto>(this ISavableTo<TDto> source)
            where TDto : class
            => source?.SaveToDto();
    }
}