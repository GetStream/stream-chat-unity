using JetBrains.Annotations;
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
            var myJObject = JObject.Parse(serializedObj);
            if (!myJObject.ContainsKey(key))
            {
                value = default;
                return false;
            }

            value = myJObject[key].Value<TValue>();
            return true;
        }
    }
}