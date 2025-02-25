using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Serialization
{
    /// <summary>
    /// Json converter to serialize and deserialize <see cref="IEnumeratedStruct"/>
    /// </summary>
    /// <typeparam name="TType">Specific type of the struct. This should be provided in a decorator</typeparam>
    internal class EnumeratedStructListConverter<TType> : JsonConverter
        where TType : struct, IEnumeratedStruct<TType>
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    return;
                case List<TType> enumeratedStructList:
                    
                    writer.WriteStartArray();
                    foreach (var enumeratedStruct in enumeratedStructList)
                    {
                        writer.WriteValue(enumeratedStruct.ToString());
                    }
                    writer.WriteEndArray();
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
                    return null;
                case JsonToken.StartArray:
                {
                    var list = new List<TType>();
                    var instance = Activator.CreateInstance<TType>();

                    var array = JArray.Load(reader);
                    foreach (var token in array)
                    {
                        if (token.Type == JTokenType.String)
                        {
                            var str = token.Value<string>();
                            if (str != null)
                            {
                                list.Add(instance.Parse(str));
                            }
                        }
                    }

                    return list;
                }
                default:
                    throw new JsonSerializationException(
                        $"Unexpected token or value when parsing {objectType.FullName}");
            }
        }

        public override bool CanConvert(Type objectType) => typeof(IEnumeratedStruct).IsAssignableFrom(objectType);
    }
}