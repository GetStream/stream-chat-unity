namespace StreamChat.Libs.Serialization
{
    /// <summary>
    /// Serializes objects to string and reverse
    /// </summary>
    public interface ISerializer
    {
        string Serialize<TType>(TType obj);

        TType Deserialize<TType>(string serializedObj);

        bool TryPeekValue<TValue>(string serializedObj, string key, out TValue value);

        object DeserializeObject(string serializedObj);
        TTargetType TryConvertTo<TTargetType>(object serializedObj);
    }
}