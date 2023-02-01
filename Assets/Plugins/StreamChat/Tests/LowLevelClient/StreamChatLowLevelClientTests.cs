#if STREAM_TESTS_ENABLED
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Configs;
using StreamChat.Core.LowLevelClient;
using StreamChat.Libs.AppInfo;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;

namespace StreamChat.Tests.LowLevelClient
{
    /// <summary>
    /// tests for <see cref="StreamChatLowLevelClient"/>
    /// </summary>
    internal class StreamChatClientTests
    {
        [SetUp]
        public void Up()
        {
            _authCredentials = new AuthCredentials("api123", "token123", "user123");
            _mockWebsocketClient = Substitute.For<IWebsocketClient>();
            _mockHttpClient = Substitute.For<IHttpClient>();
            _mockSerializer = Substitute.For<ISerializer>();
            _mockTimeService = Substitute.For<ITimeService>();
            _mockApplicationInfo = Substitute.For<IApplicationInfo>();
            _mockLogs = new UnityLogs();
            _mockStreamClientConfig = Substitute.For<IStreamClientConfig>();

            _lowLevelClient = new StreamChatLowLevelClient(_authCredentials, _mockWebsocketClient, _mockHttpClient,
                _mockSerializer, _mockTimeService, _mockApplicationInfo, _mockLogs, _mockStreamClientConfig);
            _lowLevelClient.Update(0.1f);
        }

        [TearDown]
        public void TearDown()
        {
            _lowLevelClient.Dispose();
            _lowLevelClient = null;

            for (int i = _resourcesToDispose.Count - 1; i >= 0; i--)
            {
                _resourcesToDispose[i].Dispose();
            }

            _resourcesToDispose.Clear();

            _mockWebsocketClient = null;
            _mockHttpClient = null;
            _mockSerializer = null;
            _mockTimeService = null;
            _mockLogs = null;
            _mockStreamClientConfig = null;
        }

        [Test]
        public void when_stream_client_start_expect_websockets_connect()
        {
            _lowLevelClient.Connect();
            _mockWebsocketClient.ReceivedWithAnyArgs().ConnectAsync(default);
        }

        [Test]
        public void when_stream_client_connection_failed_expect_reconnect()
        {
            _mockTimeService.Time.Returns(0);
            _mockWebsocketClient.ConnectionFailed += Raise.Event<Action>();
            _lowLevelClient.Connect();

            // Tick for client to react to WS connection failure
            _lowLevelClient.Update(0.1f);

            // Simulate 3 seconds have passed
            _mockTimeService.Time.Returns(3);

            // Tick frame for client to issue reconnect
            _lowLevelClient.Update(0.1f);

            _mockWebsocketClient.ReceivedWithAnyArgs(2).ConnectAsync(default);
        }

        [Test]
        public void when_stream_client_factory_called_expect_no_exceptions()
        {
            Assert.DoesNotThrow(() =>
            {
                var instance = StreamChatLowLevelClient.CreateDefaultClient(_authCredentials);
                _resourcesToDispose.Add(instance);
            });
        }

        [Test]
        public void when_stream_client_passed_null_arg_expect_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => new StreamChatLowLevelClient(_authCredentials,
                websocketClient: null,
                httpClient: _mockHttpClient, serializer: _mockSerializer,
                timeService: _mockTimeService, applicationInfo: _mockApplicationInfo, logs: _mockLogs,
                config: _mockStreamClientConfig));

            Assert.Throws<ArgumentNullException>(() => new StreamChatLowLevelClient(_authCredentials,
                websocketClient: _mockWebsocketClient, httpClient: null, serializer: _mockSerializer,
                timeService: _mockTimeService, applicationInfo: _mockApplicationInfo, logs: _mockLogs,
                config: _mockStreamClientConfig));

            Assert.Throws<ArgumentNullException>(() => new StreamChatLowLevelClient(_authCredentials,
                websocketClient: _mockWebsocketClient, httpClient: _mockHttpClient, serializer: null,
                timeService: _mockTimeService, applicationInfo: _mockApplicationInfo, logs: _mockLogs,
                config: _mockStreamClientConfig));

            Assert.Throws<ArgumentNullException>(() => new StreamChatLowLevelClient(_authCredentials,
                websocketClient: _mockWebsocketClient, httpClient: _mockHttpClient, serializer: _mockSerializer,
                timeService: null, logs: _mockLogs, applicationInfo: _mockApplicationInfo,
                config: _mockStreamClientConfig));

            Assert.Throws<ArgumentNullException>(() => new StreamChatLowLevelClient(_authCredentials,
                websocketClient: _mockWebsocketClient, httpClient: _mockHttpClient, serializer: _mockSerializer,
                timeService: _mockTimeService, applicationInfo: null, logs: _mockLogs,
                config: _mockStreamClientConfig));

            Assert.Throws<ArgumentNullException>(() => new StreamChatLowLevelClient(_authCredentials,
                websocketClient: _mockWebsocketClient, httpClient: _mockHttpClient, serializer: _mockSerializer,
                timeService: _mockTimeService, applicationInfo: _mockApplicationInfo, logs: null,
                config: _mockStreamClientConfig));

            Assert.Throws<ArgumentNullException>(() => new StreamChatLowLevelClient(_authCredentials,
                websocketClient: _mockWebsocketClient, httpClient: _mockHttpClient, serializer: _mockSerializer,
                timeService: _mockTimeService, applicationInfo: _mockApplicationInfo, logs: _mockLogs, config: null));
        }

        [Test]
        public void when_stream_client_created_expect_disconnected_state()
        {
            Assert.IsTrue(_lowLevelClient.ConnectionState == ConnectionState.Disconnected);
        }

        [Test]
        public void when_stream_client_received_first_health_check_event_expect_connected_state()
        {
            var client = new StreamChatLowLevelClient(_authCredentials, _mockWebsocketClient, _mockHttpClient,
                new NewtonsoftJsonSerializer(), _mockTimeService, _mockApplicationInfo, _mockLogs,
                _mockStreamClientConfig);
            _resourcesToDispose.Add(client);

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
            var client = new StreamChatLowLevelClient(_authCredentials, _mockWebsocketClient, _mockHttpClient,
                new NewtonsoftJsonSerializer(), _mockTimeService, _mockApplicationInfo, _mockLogs,
                _mockStreamClientConfig);
            _resourcesToDispose.Add(client);

            var connectCallsCounter = 0;
            _mockWebsocketClient.ConnectAsync(Arg.Any<Uri>()).Returns(_ =>
            {
                connectCallsCounter++;
                return Task.CompletedTask;
            });

            _mockWebsocketClient.When(_ => _.DisconnectAsync(Arg.Any<WebSocketCloseStatus>(), Arg.Any<string>()))
                .Do(callbackInfo => { _mockWebsocketClient.Disconnected += Raise.Event<Action>(); });

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

        private readonly List<IDisposable> _resourcesToDispose = new List<IDisposable>();

        private IStreamChatLowLevelClient _lowLevelClient;
        private AuthCredentials _authCredentials;

        private IWebsocketClient _mockWebsocketClient;
        private IApplicationInfo _mockApplicationInfo;
        private ILogs _mockLogs;
        private ISerializer _mockSerializer;
        private ITimeService _mockTimeService;
        private IHttpClient _mockHttpClient;
        private IStreamClientConfig _mockStreamClientConfig;
    }
}
#endif