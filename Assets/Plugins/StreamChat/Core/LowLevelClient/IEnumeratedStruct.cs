namespace StreamChat.Core.LowLevelClient
{
    /// <summary>
    /// <inheritdoc cref="IEnumeratedStruct"/>
    /// </summary>
    internal interface IEnumeratedStruct<out TType> : IEnumeratedStruct
        where TType : struct
    {
        TType Parse(string value);
    }
    
    /// <summary>
    /// Struct that is used to represent enumerated values
    /// </summary>
    internal interface IEnumeratedStruct
    {
        string Value { get; }
    }
}