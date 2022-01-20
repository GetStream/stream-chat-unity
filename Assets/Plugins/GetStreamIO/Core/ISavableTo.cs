namespace StreamChat.Core
{
    /// <summary>
    /// Supports saving object to DTO of a given type
    /// </summary>
    /// <typeparam name="TDto">DTO type</typeparam>
    public interface ISavableTo<out TDto>
    {
        TDto SaveToDto();
    }
}