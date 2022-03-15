using System;
using StreamChat.Core;
using StreamChat.Libs.Serialization;

namespace StreamChat.Tests.Api
{
    /// <summary>
    /// Base class for endpoint test case
    /// </summary>
    public abstract class EndpointTestCaseBase
    {
        public abstract string Name { get; }

        /// <summary>
        /// Execute high level request on <see cref="IStreamChatClient"/>
        /// </summary>
        public abstract void ExecuteRequest(IStreamChatClient client);

        /// <summary>
        /// Validation of Http Request Uri
        /// </summary>
        /// <param name="uri">Uri passed to http client</param>
        public abstract bool IsUriValid(Uri uri);

        /// <summary>
        /// Validation of Http Request Body
        /// </summary>
        /// <param name="serializedRequestBody">Serialized json content passed to http client</param>
        /// <returns></returns>
        public bool IsRequestBodyValid(string serializedRequestBody)
        {
            var deserialized = _serializer.DeserializeObject(serializedRequestBody);
            return deserialized != default && InternalIsRequestBodyValid(deserialized);
        }

        protected abstract bool InternalIsRequestBodyValid(dynamic deserializedRequestBody);

        private readonly ISerializer _serializer = new NewtonsoftJsonSerializer();
    }
}