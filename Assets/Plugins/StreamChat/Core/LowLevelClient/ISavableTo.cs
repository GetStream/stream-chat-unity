namespace StreamChat.Core.LowLevelClient
{
    /// <summary>
    /// Supports saving object to DTO of a given type
    /// </summary>
    /// <typeparam name="TDto">DTO type</typeparam>
    internal interface ISavableTo<out TDto>
    {
        TDto SaveToDto();
    }
}