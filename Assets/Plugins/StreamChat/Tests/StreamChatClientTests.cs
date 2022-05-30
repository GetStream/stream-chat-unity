#if STREAM_TESTS_ENABLED
using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;

namespace StreamChat.Tests
{
    /// <summary>
    /// tests for <see cref="StreamChatClient"/>
    /// </summary>
    public class StreamChatClientTests
    {
        [SetUp]
        public void Up()
        {
            _authCredentials = new AuthCredentials("api123", "token123", "user123");
            _mockWebsocketClient = Substitute.For<IWebsocketClient>();
            _mockHttpClient = Substitute.For<IHttpClient>();
            _mockSerializer = Substitute.For<ISerializer>();
            _mockTimeService = Substitute.For<ITimeService>();
            _mockLogs = Substitute.For<ILogs>();

            _client = new StreamChatClient(_authCredentials, _mockWebsocketClient, _mockHttpClient, _mockSerializer,
                _mockTimeService, _mockLogs);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _client = null;

            _mockWebsocketClient = null;
            _mockHttpClient = null;
            _mockSerializer = null;
            _mockTimeService = null;
            _mockLogs = null;
        }

        [Test]
        public void when_stream_client_start_expect_websockets_connect()
        {
            _client.Connect();
            _mockWebsocketClient.ReceivedWithAnyArgs().ConnectAsync(default);
        }

        [Test]
        public void when_stream_client_connection_failed_expect_reconnect()
        {
            _client.Connect();
            _mockWebsocketClient.ConnectionFailed += Raise.Event<Action>();

            _mockWebsocketClient.ReceivedWithAnyArgs(2).ConnectAsync(default);
        }

        [Test]
        public void when_stream_client_max_reconnections_reached_expect_disconnected_state()
        {
            _client.Connect();

            for (int i = 0; i < StreamChatClient.ReconnectMaxAttempts + 10; i++)
            {
                _mockWebsocketClient.ConnectionFailed += Raise.Event<Action>();
            }

            var expectedAttempts = StreamChatClient.ReconnectMaxAttempts + 1;

            _mockWebsocketClient.ReceivedWithAnyArgs(expectedAttempts).ConnectAsync(default);

            Assert.AreEqual(_client.ConnectionState, ConnectionState.Disconnected);
        }

        [Test]
        public void when_stream_client_factory_called_expect_no_exceptions()
        {
            Assert.DoesNotThrow(() => StreamChatClient.CreateDefaultClient(_authCredentials));
        }

        [Test]
        public void when_stream_client_passed_null_arg_expect_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => new StreamChatClient(_authCredentials, websocketClient: null, httpClient: _mockHttpClient, serializer: _mockSerializer,
                timeService: _mockTimeService, logs: _mockLogs));

            Assert.Throws<ArgumentNullException>(() => new StreamChatClient(_authCredentials,
                websocketClient: _mockWebsocketClient, httpClient: null, serializer: _mockSerializer,
                timeService: _mockTimeService, logs: _mockLogs));

            Assert.Throws<ArgumentNullException>(() => new StreamChatClient(_authCredentials,
                websocketClient: _mockWebsocketClient, httpClient: _mockHttpClient, serializer: null,
                timeService: _mockTimeService, logs: _mockLogs));

            Assert.Throws<ArgumentNullException>(() => new StreamChatClient(_authCredentials,
                websocketClient: _mockWebsocketClient, httpClient: _mockHttpClient, serializer: _mockSerializer,
                timeService: null, logs: _mockLogs));

            Assert.Throws<ArgumentNullException>(() => new StreamChatClient(_authCredentials,
                websocketClient: _mockWebsocketClient, httpClient: _mockHttpClient, serializer: _mockSerializer,
                timeService: _mockTimeService, logs: null));
        }

        [Test]
        public void when_stream_client_created_expect_disconnected_state()
        {
            Assert.IsTrue(_client.ConnectionState == ConnectionState.Disconnected);
        }

        [Test]
        public void when_stream_client_received_first_health_check_event_expect_connected_state()
        {
            var client = new StreamChatClient(_authCredentials, _mockWebsocketClient, _mockHttpClient,
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

            client.Connect();
            client.Update(deltaTime: 0.2f);

            Assert.IsTrue(client.ConnectionState == ConnectionState.Connected);
        }

        [Test]
        public void when_stream_client_health_check_timeout_detected_expect_client_disconnected()
        {
            var client = new StreamChatClient(_authCredentials, _mockWebsocketClient, _mockHttpClient,
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

            client.Connect();
            client.Update(deltaTime: 0.2f);
            _mockTimeService.Time.Returns(31);
            client.Update(0.2f);

            Assert.IsFalse(client.ConnectionState == ConnectionState.Connected);
        }

        private IStreamChatClient _client;
        private AuthCredentials _authCredentials;

        private IWebsocketClient _mockWebsocketClient;
        private ILogs _mockLogs;
        private ISerializer _mockSerializer;
        private ITimeService _mockTimeService;
        private IHttpClient _mockHttpClient;
    }
}
#endif