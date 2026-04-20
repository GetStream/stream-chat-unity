using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StreamChat.Libs.Serialization;

namespace StreamChat.Tests.Runtime
{
    /// <summary>
    /// Runtime serialization smoke tests for the entire DTO layer.
    /// These must run in an IL2CPP player build to catch reflection/stripping issues
    /// that don't surface under Mono in the editor.
    /// </summary>
    internal class DTOSerializationTests
    {
        private static readonly string[] DTONamespaces =
        {
            "StreamChat.Core.InternalDTO.Models",
            "StreamChat.Core.InternalDTO.Requests",
            "StreamChat.Core.InternalDTO.Responses",
            "StreamChat.Core.InternalDTO.Events",
        };

        /// <summary>
        /// Create a default instance of every DTO class, serialize it, and deserialize
        /// the result back. This exercises the full Newtonsoft.Json contract for each type
        /// including all [JsonProperty] and [JsonConverter] attributes, catching IL2CPP
        /// reflection and code-stripping issues.
        /// </summary>
        [Test]
        public void All_DTO_types_survive_serialize_deserialize_round_trip()
        {
            var serializer = new NewtonsoftJsonSerializer();
            var dtoTypes = GetDTOClassTypes();

            var serializeMethod = typeof(NewtonsoftJsonSerializer).GetMethod("Serialize");
            var deserializeMethod = typeof(NewtonsoftJsonSerializer).GetMethod("Deserialize");

            var errors = new StringBuilder();

            foreach (var type in dtoTypes)
            {
                try
                {
                    var instance = Activator.CreateInstance(type);

                    var json = (string)serializeMethod
                        .MakeGenericMethod(type)
                        .Invoke(serializer, new[] { instance });

                    deserializeMethod
                        .MakeGenericMethod(type)
                        .Invoke(serializer, new object[] { json });
                }
                catch (Exception e)
                {
                    var inner = e.InnerException ?? e;
                    errors.AppendLine($"{type.Name}: {inner.GetType().Name}: {inner.Message}");
                }
            }

            Assert.AreEqual(0, errors.Length, errors.ToString());
        }

        /// <summary>
        /// Deserialize an empty JSON object into every DTO class.
        /// Newtonsoft.Json builds the full property contract (processes all attributes) on
        /// first encounter, so this catches converter instantiation failures even though
        /// no property values are present in the payload.
        /// </summary>
        [Test]
        public void All_DTO_types_can_be_deserialized_from_empty_json()
        {
            var serializer = new NewtonsoftJsonSerializer();
            var dtoTypes = GetDTOClassTypes();

            var deserializeMethod = typeof(NewtonsoftJsonSerializer).GetMethod("Deserialize");

            var errors = new StringBuilder();

            foreach (var type in dtoTypes)
            {
                try
                {
                    deserializeMethod
                        .MakeGenericMethod(type)
                        .Invoke(serializer, new object[] { "{}" });
                }
                catch (Exception e)
                {
                    var inner = e.InnerException ?? e;
                    errors.AppendLine($"{type.Name}: {inner.GetType().Name}: {inner.Message}");
                }
            }

            Assert.AreEqual(0, errors.Length, errors.ToString());
        }

        private static Type[] GetDTOClassTypes()
        {
            var coreAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .Single(a => a.GetName().Name == "StreamChat.Core");

            return coreAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType)
                .Where(t => t.Namespace != null && DTONamespaces.Contains(t.Namespace))
                .ToArray();
        }
    }
}
