#if STREAM_TESTS_ENABLED
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using NSubstitute;
using NUnit.Framework;
using StreamChat.Core.Configs;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.API;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Libs.AppInfo;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.NetworkMonitors;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;

namespace StreamChat.Tests.LowLevelClient.Api
{
    /// <summary>
    /// Tests for <see cref="IMessageApi"/> client
    /// </summary>
    internal class MessageApiTests
    {
        [SetUp]
        public void Up()
        {
            _authCredentials = new AuthCredentials("api123", "token123", "user123");
            _mockWebsocketClient = Substitute.For<IWebsocketClient>();
            _mockHttpClient = Substitute.For<IHttpClient>();
            _serializer = new NewtonsoftJsonSerializer();
            _mockTimeService = Substitute.For<ITimeService>();
            _mockNetworkMonitor = Substitute.For<INetworkMonitor>();
            _mockApplicationInfo = Substitute.For<IApplicationInfo>();
            _mockLogs = Substitute.For<ILogs>();
            _mockStreamClientConfig = Substitute.For<IStreamClientConfig>();

            _lowLevelClient = new StreamChatLowLevelClient(_authCredentials, _mockWebsocketClient, _mockHttpClient, _serializer,
                _mockTimeService, _mockNetworkMonitor, _mockApplicationInfo, _mockLogs, _mockStreamClientConfig);
        }

        [TearDown]
        public void TearDown()
        {
            _lowLevelClient.Dispose();
            _lowLevelClient = null;

            _mockWebsocketClient = null;
            _serializer = null;
            _mockTimeService = null;
            _mockLogs = null;
        }

        [TestCaseSource(nameof(GetPostTestCases))]
        public void when_client_post_request_expect_valid_uri_and_request_body_in_http_client(EndpointTestCaseBase testCase)
        {
            _lowLevelClient.Connect();

            var response = new HttpResponse(true, 200, "{\"reaction\": {\"type\": \"like\"}}");

            _mockHttpClient.SendHttpRequestAsync(Arg.Is(HttpMethodType.Post),Arg.Any<Uri>(), Arg.Any<object>())
                .Returns(response);

            testCase.ExecuteRequest(_lowLevelClient);

            Expression<Predicate<Uri>> ValidateUri = uri => testCase.IsUriValid(uri);
            Expression<Predicate<object>> ValidateRequestBody = request => testCase.IsRequestBodyValid(request as string);

            _mockHttpClient.Received().SendHttpRequestAsync(Arg.Is(HttpMethodType.Post),Arg.Is(ValidateUri), Arg.Is(ValidateRequestBody));
        }

        [TestCaseSource(nameof(GetDeleteTestCases))]
        public void when_client_delete_request_expect_valid_uri_in_http_client(EndpointTestCaseBase testCase)
        {
            _lowLevelClient.Connect();
            
            var response = new HttpResponse(true, 200, "{\"reaction\": {\"type\": \"like\"}}");

            _mockHttpClient.SendHttpRequestAsync(Arg.Is(HttpMethodType.Post),Arg.Any<Uri>(), Arg.Any<HttpContent>())
                .Returns(response);

            testCase.ExecuteRequest(_lowLevelClient);

            Expression<Predicate<Uri>> ValidateUri = uri => testCase.IsUriValid(uri);

            _mockHttpClient.Received().SendHttpRequestAsync(Arg.Is(HttpMethodType.Delete), Arg.Is(ValidateUri), Arg.Any<object>());
        }

        private IStreamChatLowLevelClient _lowLevelClient;
        private AuthCredentials _authCredentials;

        private IWebsocketClient _mockWebsocketClient;
        private ITimeService _mockTimeService;
        private IHttpClient _mockHttpClient;
        private ILogs _mockLogs;
        private INetworkMonitor _mockNetworkMonitor;
        private IApplicationInfo _mockApplicationInfo;
        private NewtonsoftJsonSerializer _serializer;
        private IStreamClientConfig _mockStreamClientConfig;

        private static readonly string NameBase = $"{nameof(IStreamChatLowLevelClient)} - {nameof(IMessageApi)}";

        private static IEnumerable<TestCaseData> GetPostTestCases
        {
            get
            {
                var sendReaction = new SendReactionTestCase();

                yield return new TestCaseData(sendReaction).SetName(sendReaction.Name);
            }
        }

        private static IEnumerable<TestCaseData> GetDeleteTestCases
        {
            get
            {
                var deleteReaction = new DeleteReactionTestCase();

                yield return new TestCaseData(deleteReaction).SetName(deleteReaction.Name);
            }
        }

        private class SendReactionTestCase : EndpointTestCaseBase
        {
            private const string MessageId = "message-id-1";
            private const string ReactionType = "like";

            public override string Name => $"{NameBase} - {nameof(IMessageApi.SendReactionAsync)}";

            public override void ExecuteRequest(IStreamChatLowLevelClient lowLevelClient)
            {
                lowLevelClient.MessageApi.SendReactionAsync(MessageId, new SendReactionRequest
                {
                    Reaction = new ReactionRequest
                    {
                        Type = ReactionType,
                    }
                });
            }

            public override bool IsUriValid(Uri uri)
                => uri.LocalPath.Equals($"/messages/{MessageId}/reaction");

            protected override bool InternalIsRequestBodyValid(dynamic requestBody)
                => requestBody.reaction.type.ToString().Equals(ReactionType);
        }

        private class DeleteReactionTestCase : EndpointTestCaseBase
        {
            private const string MessageId = "message-id-1";
            private const string ReactionType = "like";

            public override string Name => $"{NameBase} - {nameof(IMessageApi.DeleteReactionAsync)}";

            public override void ExecuteRequest(IStreamChatLowLevelClient lowLevelClient)
            {
                lowLevelClient.MessageApi.DeleteReactionAsync(MessageId, ReactionType);
            }

            public override bool IsUriValid(Uri uri)
                => uri.LocalPath.Equals($"/messages/{MessageId}/reaction/{ReactionType}");

            protected override bool InternalIsRequestBodyValid(dynamic requestBody)
                => true;
        }
    }
}
#endif