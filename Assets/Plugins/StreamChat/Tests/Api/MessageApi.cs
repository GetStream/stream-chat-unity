using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NSubstitute;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.API;
using StreamChat.Core.Requests;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;

namespace StreamChat.Tests.Api
{
    /// <summary>
    /// Tests for <see cref="IMessageApi"/> client
    /// </summary>
    public class MessageApiTests
    {
        [SetUp]
        public void Up()
        {
            _authCredentials = new AuthCredentials("api123", "token123", "user123");
            _mockWebsocketClient = Substitute.For<IWebsocketClient>();
            _mockHttpClient = Substitute.For<IHttpClient>();
            _serializer = new NewtonsoftJsonSerializer();
            _mockTimeService = Substitute.For<ITimeService>();
            _mockLogs = Substitute.For<ILogs>();

            _client = new StreamChatClient(_authCredentials, _mockWebsocketClient, _mockHttpClient, _serializer,
                _mockTimeService, _mockLogs);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _client = null;

            _mockWebsocketClient = null;
            _serializer = null;
            _mockTimeService = null;
            _mockLogs = null;
        }

        [TestCaseSource(nameof(GetPostTestCases))]
        public void when_client_post_request_expect_valid_uri_and_request_body_in_http_client(EndpointTestCaseBase testCase)
        {
            _client.Connect();

            testCase.ExecuteRequest(_client);

            Expression<Predicate<Uri>> ValidateUri = uri => testCase.IsUriValid(uri);
            Expression<Predicate<string>> ValidateRequestBody = request => testCase.IsRequestBodyValid(request);

            _mockHttpClient.Received().PostAsync(Arg.Is(ValidateUri), Arg.Is(ValidateRequestBody));
        }

        [TestCaseSource(nameof(GetDeleteTestCases))]
        public void when_client_delete_request_expect_valid_uri_in_http_client(EndpointTestCaseBase testCase)
        {
            _client.Connect();

            testCase.ExecuteRequest(_client);

            Expression<Predicate<Uri>> ValidateUri = uri => testCase.IsUriValid(uri);

            _mockHttpClient.Received().DeleteAsync(Arg.Is(ValidateUri));
        }

        private IStreamChatClient _client;
        private AuthCredentials _authCredentials;

        private IWebsocketClient _mockWebsocketClient;
        private ILogs _mockLogs;
        private ITimeService _mockTimeService;
        private IHttpClient _mockHttpClient;
        private NewtonsoftJsonSerializer _serializer;

        private static readonly string NameBase = $"{nameof(IStreamChatClient)} - {nameof(IMessageApi)}";

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

            public override void ExecuteRequest(IStreamChatClient client)
            {
                client.MessageApi.SendReactionAsync(MessageId, new SendReactionRequest
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

            public override void ExecuteRequest(IStreamChatClient client)
            {
                client.MessageApi.DeleteReactionAsync(MessageId, ReactionType);
            }

            public override bool IsUriValid(Uri uri)
                => uri.LocalPath.Equals($"/messages/{MessageId}/reaction/{ReactionType}");

            protected override bool InternalIsRequestBodyValid(dynamic requestBody)
                => true;
        }
    }
}