using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Plugins.GetStreamIO.Core;
using Plugins.GetStreamIO.Core.Auth;
using Plugins.GetStreamIO.Libs.Http;
using Plugins.GetStreamIO.Libs.Logs;
using Plugins.GetStreamIO.Libs.Serialization;
using Plugins.GetStreamIO.Libs.Time;
using Plugins.GetStreamIO.Libs.Websockets;

namespace Plugins.GetStreamIO.Tests
{
    /// <summary>
    /// tests for <see cref="GetStreamChatClient"/>
    /// </summary>
    public class GetStreamChatClientTests
    {
        [SetUp]
        public void Up()
        {
            _authData = new AuthData(new Uri("wss://test.com"), "token123", "api123", "user123");
            _mockWebsocketClient = Substitute.For<IWebsocketClient>();
            _mockHttpClient = Substitute.For<IHttpClient>();
            _mockSerializer = Substitute.For<ISerializer>();
            _mockTimeService = Substitute.For<ITimeService>();
            _mockLogs = Substitute.For<ILogs>();
            _client = new GetStreamChatClient(_authData, _mockWebsocketClient, _mockHttpClient, _mockSerializer,
                _mockTimeService, _mockLogs);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _client = null;

            _mockWebsocketClient = null;
            _mockSerializer = null;
            _mockTimeService = null;
            _mockLogs = null;
        }

        [Test]
        public void when_stream_client_start_expect_websockets_connect()
        {
            _client.Start();
            _mockWebsocketClient.ReceivedWithAnyArgs().ConnectAsync(default);
        }

        [Test]
        public void when_get_stream_client_connection_failed_expect_reconnect()
        {
            _mockWebsocketClient.ConnectAsync(default)
                .ReturnsForAnyArgs(_ => Task.FromException<Exception>(new Exception("failed to connect")));

            _client.Start();
            _mockWebsocketClient.ReceivedWithAnyArgs().ConnectAsync(default);
        }

        [Test]
        public void when_get_stream_client_factory_called_expect_no_exceptions()
        {
            Assert.DoesNotThrow(() => GetStreamChatClient.Factory(_authData));
        }

        [Test]
        public void when_get_stream_client_passed_null_arg_expect_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => new GetStreamChatClient(_authData, websocketClient: null,
                serializer: _mockSerializer, httpClient: _mockHttpClient, timeService: _mockTimeService,
                logs: _mockLogs));

            Assert.Throws<ArgumentNullException>(() => new GetStreamChatClient(_authData,
                websocketClient: _mockWebsocketClient, httpClient: null, serializer: _mockSerializer,
                timeService: _mockTimeService, logs: _mockLogs));

            Assert.Throws<ArgumentNullException>(() => new GetStreamChatClient(_authData,
                websocketClient: _mockWebsocketClient, httpClient: _mockHttpClient, serializer: null,
                timeService: _mockTimeService, logs: _mockLogs));

            Assert.Throws<ArgumentNullException>(() => new GetStreamChatClient(_authData,
                websocketClient: _mockWebsocketClient, httpClient: _mockHttpClient, serializer: _mockSerializer,
                timeService: null, logs: _mockLogs));

            Assert.Throws<ArgumentNullException>(() => new GetStreamChatClient(_authData,
                websocketClient: _mockWebsocketClient, httpClient: _mockHttpClient, serializer: _mockSerializer,
                timeService: _mockTimeService, logs: null));
        }

        [Test]
        public void when_get_stream_client_created_expect_disconnected_state()
        {
            Assert.IsTrue(_client.ConnectionState == ConnectionState.Disconnected);
        }

        [Test]
        public void when_get_stream_client_received_first_health_check_event_expect_connected_state()
        {
            var client = new GetStreamChatClient(_authData, _mockWebsocketClient, _mockHttpClient,
                new NewtonsoftJsonSerializer(), _mockTimeService, _mockLogs);

            var connectCallsCounter = 0;
            _mockWebsocketClient.ConnectAsync(Arg.Any<Uri>()).Returns(_ =>
            {
                connectCallsCounter++;
                return Task.CompletedTask;
            });

            _mockWebsocketClient.TryDequeueMessage(out Arg.Any<string>()).Returns(arg =>
            {
                arg[0] = "{\"connection_id\":\"fakeId\", \"type\":\"health.check\"}";
                return true;
            }, arg => false);

            client.Start();
            client.Update(deltaTime: 0.2f);

            Assert.IsTrue(client.ConnectionState == ConnectionState.Connected);
        }

        [Test]
        public void when_get_stream_client_health_check_timeout_detected_expect_client_disconnected()
        {
            var client = new GetStreamChatClient(_authData, _mockWebsocketClient, _mockHttpClient,
                new NewtonsoftJsonSerializer(), _mockTimeService, _mockLogs);

            var connectCallsCounter = 0;
            _mockWebsocketClient.ConnectAsync(Arg.Any<Uri>()).Returns(_ =>
            {
                connectCallsCounter++;
                return Task.CompletedTask;
            });

            _mockWebsocketClient.TryDequeueMessage(out Arg.Any<string>()).Returns(arg =>
            {
                arg[0] = "{\"connection_id\":\"fakeId\", \"type\":\"health.check\"}";
                return true;
            }, arg => false);

            client.Start();
            client.Update(deltaTime: 0.2f);
            _mockTimeService.Time.Returns(31);
            client.Update(0.2f);

            Assert.IsFalse(client.ConnectionState == ConnectionState.Connected);
        }

        private IGetStreamChatClient _client;
        private IWebsocketClient _mockWebsocketClient;
        private AuthData _authData;
        private ILogs _mockLogs;
        private ISerializer _mockSerializer;
        private ITimeService _mockTimeService;
        private IHttpClient _mockHttpClient;
    }
}