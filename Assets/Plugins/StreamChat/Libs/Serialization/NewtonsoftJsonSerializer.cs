using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat.Libs.Serialization
{
    /// <summary>
    /// https://www.newtonsoft.com/ Json implementation of <see cref="ISerializer"/>
    /// </summary>
    public class NewtonsoftJsonSerializer : ISerializer
    {
        public string Serialize<TType>(TType obj)
            => JsonConvert.SerializeObject(obj, Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                });

        public TType Deserialize<TType>(string serializedObj)
            => JsonConvert.DeserializeObject<TType>(serializedObj);

        public object DeserializeObject(string serializedObj)
            => JsonConvert.DeserializeObject(serializedObj);

        public bool TryPeekValue<TValue>(string serializedObj, string key, out TValue value)
        {
            var wrapperJObject = JObject.Parse(serializedObj);
            if (!wrapperJObject.ContainsKey(key))
            {
                value = default;
                return false;
            }

            var obj = wrapperJObject[key];

            if (obj is JObject childJObject)
            {
                value = childJObject.ToObject<TValue>();
                return true;
            }

            if (obj is JToken childToken)
            {
                value = childToken.Value<TValue>();
                return true;
            }

            throw new Exception("Unhandled object type: " + obj.GetType());
        }
    }
}