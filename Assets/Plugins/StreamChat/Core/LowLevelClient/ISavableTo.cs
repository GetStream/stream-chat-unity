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

    /// <summary>
    /// Secondary DTO save target. Identical to <see cref="ISavableTo{TDto}"/> but declared as a separate interface
    /// to work around an IL2CPP vtable builder bug (Unity 6000.0.x) that crashes when a single type implements
    /// multiple closed versions of the same generic interface.
    /// </summary>
    internal interface ISavableTo2<out TDto>
    {
        TDto SaveToDto();
    }

    /// <summary>
    /// Tertiary DTO save target. Same workaround as <see cref="ISavableTo2{TDto}"/> for types that need
    /// three distinct DTO conversions (e.g. <see cref="Requests.UserObjectRequest"/>).
    /// </summary>
    internal interface ISavableTo3<out TDto>
    {
        TDto SaveToDto();
    }
}