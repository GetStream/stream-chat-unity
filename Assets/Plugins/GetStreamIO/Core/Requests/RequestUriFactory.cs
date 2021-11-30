using System;
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
        public RequestUriFactory(IAuthProvider authProvider, IConnectionProvider connectionProvider, ISerializer serializer)
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
                {"json", Uri.EscapeDataString(serializedPayload)},
                {"api_key", _authProvider.ApiKey},
                {"authorization", _authProvider.UserToken},
                {"stream-auth-type", _authProvider.StreamAuthType},
            };

            var uriBuilder = new UriBuilder(_connectionProvider.ServerUri)
                {Path = "connect", Query = uriParams.ToQueryParams()};

            return uriBuilder.Uri;
        }

        public Uri CreateChannelsUri()
        {
            var uriBuilder = new UriBuilder(_connectionProvider.ServerUri)
                {Path = "channels", Scheme = "https", Query = GetDefaultParamsQuery()};

            return uriBuilder.Uri;
        }

        public Uri CreateSendMessageUri(Channel channel)
        {
            var path = $"/channels/messaging/{channel.Details.Id}/message";
            var uriBuilder = new UriBuilder(_connectionProvider.ServerUri)
                {Path = path, Scheme = "https", Query = GetDefaultParamsQuery()};

            return uriBuilder.Uri;
        }

        private readonly IAuthProvider _authProvider;
        private readonly ISerializer _serializer;
        private readonly IConnectionProvider _connectionProvider;

        private string GetDefaultParamsQuery()
        {
            var uriParams = new Dictionary<string, string>
            {
                {"user_id", _authProvider.UserId},
                {"api_key", _authProvider.ApiKey},
                {"connection_id", _connectionProvider.ConnectionId},
            };

            return uriParams.ToQueryParams();
        }

    }
}