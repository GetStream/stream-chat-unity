using System;
using Newtonsoft.Json;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Serialization
{
    /// <summary>
    /// Json converter to serialize and deserialize <see cref="IEnumeratedStruct"/>
    /// </summary>
    /// <typeparam name="TType">Specific type of the struct. This should be provided in a decorator</typeparam>
    internal class EnumeratedStructConverter<TType> : JsonConverter
        where TType : struct, IEnumeratedStruct<TType>
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    return;
                case TType enumeratedStruct:
                    writer.WriteValue(enumeratedStruct.ToString());
                    break;
                default:
                    throw new JsonSerializationException($"Unexpected value type: {value.GetType()}");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return default(TType);
                case JsonToken.String:
                {
                    var str = reader.Value?.ToString();
                    if (str == null)
                    {
                        return default(TType);
                    }

                    var instance = Activator.CreateInstance<TType>();
                    return instance.Parse(str);
                }
                default:
                    throw new JsonSerializationException(
                        $"Unexpected token or value when parsing {objectType.FullName}");
            }
        }

        public override bool CanConvert(Type objectType) => typeof(IEnumeratedStruct).IsAssignableFrom(objectType);
    }
}