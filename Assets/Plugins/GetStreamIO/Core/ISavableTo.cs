namespace Plugins.GetStreamIO.Core
{
    /// <summary>
    /// Supports saving object to DTO of a given type
    /// </summary>
    /// <typeparam name="TDto">DTO type</typeparam>
    public interface ISavableTo<out TDto>
    {
        TDto SaveToDto();
    }

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