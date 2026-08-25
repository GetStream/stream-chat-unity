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
using StreamChat.Libs.ChatInstanceRunner;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.NetworkMonitors;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;

namespace StreamChat.Tests.LowLevelClient
{
    internal class StreamChatClientLifecycleTests
    {
        [SetUp]
        public void Up()
        {
            _authCredentials = new AuthCredentials("api123", "user123", "token123");
            _mockWebsocketClient = Substitute.For<IWebsocketClient>();
            _mockHttpClient = Substitute.For<IHttpClient>();
            _mockTimeService = Substitute.For<ITimeService>();
            _mockNetworkMonitor = Substitute.For<INetworkMonitor>();
            _mockApplicationInfo = Substitute.For<IApplicationInfo>();
            _mockLogs = Substitute.For<ILogs>();
            _config = new StreamClientConfig { DisconnectOnApplicationPause = true };

            _mockWebsocketClient.ConnectAsync(Arg.Any<Uri>()).Returns(Task.CompletedTask);
            _mockWebsocketClient.When(_ => _.DisconnectAsync(Arg.Any<WebSocketCloseStatus>(), Arg.Any<string>()))
                .Do(_ => { _mockWebsocketClient.Disconnected += Raise.Event<Action>(); });
            EnqueueHealthCheckOnce();
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = _resourcesToDispose.Count - 1; i >= 0; i--)
            {
                _resourcesToDispose[i].Dispose();
            }

            _resourcesToDispose.Clear();
        }

        [Test]
        public void when_pause_connection_expect_disconnected_and_scheduler_armed()
        {
            var client = CreateConnectedClient();
            _mockTimeService.Time.Returns(10);

            client.PauseConnectionAsync().GetAwaiter().GetResult();

            Assert.AreEqual(ConnectionState.Disconnected, client.ConnectionState);
            Assert.AreEqual(DisconnectCause.ConnectionReleased, client.InternalLowLevelClient.LastDisconnectCause);
            Assert.AreEqual(10, client.NextReconnectTime.Value);
            Assert.AreNotEqual((double)float.MaxValue, client.NextReconnectTime.Value);
        }

        [Test]
        public void when_pause_connection_then_resume_expect_connect_called()
        {
            var client = CreateConnectedClient();
            _mockTimeService.Time.Returns(10);

            client.PauseConnectionAsync().GetAwaiter().GetResult();
            client.ResumeConnectionAsync().GetAwaiter().GetResult();

            Assert.AreEqual(ConnectionState.Connecting, client.ConnectionState);
            _mockWebsocketClient.ReceivedWithAnyArgs(2).ConnectAsync(default);
        }

        [Test]
        public void when_pause_connection_then_update_expect_reconnect()
        {
            var client = CreateConnectedClient();
            _mockTimeService.Time.Returns(10);

            client.PauseConnectionAsync().GetAwaiter().GetResult();
            ((IStreamChatClientEventsListener)client).Update();

            _mockWebsocketClient.ReceivedWithAnyArgs(2).ConnectAsync(default);
        }

        [Test]
        public void when_disconnect_user_expect_scheduler_stopped_and_no_reconnect_on_update()
        {
            var client = CreateConnectedClient();
            _mockTimeService.Time.Returns(10);

            client.DisconnectUserAsync().GetAwaiter().GetResult();
            ((IStreamChatClientEventsListener)client).Update();

            Assert.AreEqual(ConnectionState.Disconnected, client.ConnectionState);
            Assert.AreEqual(DisconnectCause.UserLogout, client.InternalLowLevelClient.LastDisconnectCause);
            Assert.AreEqual(float.MaxValue, client.NextReconnectTime.Value);
            _mockWebsocketClient.ReceivedWithAnyArgs(1).ConnectAsync(default);
        }

        [Test]
        public void when_application_paused_expect_socket_closed_with_pause_cause()
        {
            var client = CreateConnectedClient();
            _mockTimeService.Time.Returns(10);

            ((IStreamChatClientEventsListener)client).OnApplicationPause(true);

            Assert.AreEqual(ConnectionState.Disconnected, client.ConnectionState);
            Assert.AreEqual(DisconnectCause.ApplicationPause, client.InternalLowLevelClient.LastDisconnectCause);
            Assert.AreNotEqual((double)float.MaxValue, client.NextReconnectTime.Value);
        }

        [Test]
        public void when_application_resumed_after_pause_expect_connect()
        {
            var client = CreateConnectedClient();
            _mockTimeService.Time.Returns(10);

            ((IStreamChatClientEventsListener)client).OnApplicationPause(true);
            ((IStreamChatClientEventsListener)client).OnApplicationPause(false);

            Assert.AreEqual(ConnectionState.Connecting, client.ConnectionState);
            _mockWebsocketClient.ReceivedWithAnyArgs(2).ConnectAsync(default);
        }

        [Test]
        public void when_pause_disconnect_disabled_expect_pause_does_not_close_socket()
        {
            _config.DisconnectOnApplicationPause = false;
            var client = CreateConnectedClient();

            ((IStreamChatClientEventsListener)client).OnApplicationPause(true);

            Assert.AreEqual(ConnectionState.Connected, client.ConnectionState);
        }

        [Test]
        public void when_application_resume_before_connect_user_expect_no_throw()
        {
            var client = CreateClient();

            Assert.DoesNotThrow(() => ((IStreamChatClientEventsListener)client).OnApplicationPause(false));
            _mockWebsocketClient.DidNotReceiveWithAnyArgs().ConnectAsync(default);
        }

        [Test]
        public void when_config_default_expect_pause_disconnect_on()
        {
            Assert.IsTrue(new StreamClientConfig().DisconnectOnApplicationPause);
        }

        private readonly List<IDisposable> _resourcesToDispose = new List<IDisposable>();

        private AuthCredentials _authCredentials;
        private IWebsocketClient _mockWebsocketClient;
        private IHttpClient _mockHttpClient;
        private ITimeService _mockTimeService;
        private INetworkMonitor _mockNetworkMonitor;
        private IApplicationInfo _mockApplicationInfo;
        private ILogs _mockLogs;
        private StreamClientConfig _config;

        private StreamChatClient CreateClient()
        {
            var client = StreamChatClient.CreateClientWithCustomDependencies(_mockWebsocketClient, _mockHttpClient,
                new NewtonsoftJsonSerializer(), _mockTimeService, _mockNetworkMonitor, _mockApplicationInfo, _mockLogs,
                _config);
            _resourcesToDispose.Add(client);
            return (StreamChatClient)client;
        }

        private StreamChatClient CreateConnectedClient()
        {
            var client = CreateClient();
            client.ConnectUserAsync(_authCredentials);
            ((IStreamChatClientEventsListener)client).Update();
            Assert.AreEqual(ConnectionState.Connected, client.ConnectionState);
            return client;
        }

        private void EnqueueHealthCheckOnce()
        {
            _mockWebsocketClient.TryDequeueMessage(out Arg.Any<string>()).Returns(arg =>
            {
                arg[0] = "{\"connection_id\":\"fakeId\", \"type\":\"health.check\"}";
                return true;
            }, arg => false);
        }
    }
}
#endif
