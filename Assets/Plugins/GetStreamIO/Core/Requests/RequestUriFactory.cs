using System;
using System.Collections;
using System.Collections.Generic;
using Plugins.GetStreamIO.Core.Auth;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Requests.DTO;
using Plugins.GetStreamIO.Libs.Serialization;
using Plugins.GetStreamIO.Libs.Utils;

namespace Plugins.GetStreamIO.Core.Requests
{
    /// <summary>
    /// Requests Uri Factory
    /// </summary>
    public class RequestUriFactory : IRequestUriFactory
    {
        public RequestUriFactory(IAuthProvider authProvider, IConnectionProvider connectionProvider,
            ISerializer serializer)
        {
            _authProvider = authProvider ?? throw new ArgumentNullException(nameof(authProvider));
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Uri CreateConnectionUri()
        {
            var connectPayloadDTO = new ConnectPayload
            {
                UserId = _authProvider.UserId,
                User = new User
                {
                    Id = _authProvider.UserId
                },
                UserToken = _authProvider.UserToken,
                ServerDeterminesConnectionId = true
            };

            var serializedPayload = _serializer.Serialize(connectPayloadDTO);

            var uriParams = new Dictionary<string, string>
            {
                { "json", Uri.EscapeDataString(serializedPayload) },
                { "api_key", _authProvider.ApiKey },
                { "authorization", _authProvider.UserToken },
                { "stream-auth-type", _authProvider.StreamAuthType },
            };

            var uriBuilder = new UriBuilder(_connectionProvider.ServerUri)
                { Path = "connect", Query = uriParams.ToQueryParameters() };

            return uriBuilder.Uri;
        }

        public Uri CreateDefaultEndpointUri(string endpoint)
            => CreateRequestUri(endpoint, GetDefaultParameters());


        public Uri CreateUpdateMessageUri(Message message)
        {
            var endPoint = $"/messages/{message.Id}";
            return CreateRequestUri(endPoint, GetDefaultParameters());
        }

        public Uri CreateDeleteMessageUri(Message message, bool? isHardDelete)
        {
            var endPoint = $"/messages/{message.Id}";
            var parameters = GetDefaultParameters();

            if (isHardDelete.HasValue)
            {
                parameters.Add("hard", isHardDelete.Value.ToString());
            }

            return CreateRequestUri(endPoint, parameters);
        }

        public Uri CreateMuteUserUri()
        {
            var endPoint = "/moderation/mute";
            return CreateRequestUri(endPoint, GetDefaultParameters());
        }

        private readonly IAuthProvider _authProvider;
        private readonly ISerializer _serializer;
        private readonly IConnectionProvider _connectionProvider;

        private Dictionary<string, string> GetDefaultParameters() =>
            new Dictionary<string, string>
            {
                { "user_id", _authProvider.UserId },
                { "api_key", _authProvider.ApiKey },
                { "connection_id", _connectionProvider.ConnectionId },
            };

        private Uri CreateRequestUri(string endPoint, IDictionary<string, string> parameters)
            => CreateRequestUri(endPoint, parameters.ToQueryParameters());

        private Uri CreateRequestUri(string endPoint, string query)
        {
            var uriBuilder = new UriBuilder(_connectionProvider.ServerUri)
                { Path = endPoint, Scheme = "https", Query = query };

            return uriBuilder.Uri;
        }
    }
}