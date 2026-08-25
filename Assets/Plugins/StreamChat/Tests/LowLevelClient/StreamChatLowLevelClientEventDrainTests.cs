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
using StreamChat.Core.LowLevelClient.Events;
using StreamChat.Libs.AppInfo;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.NetworkMonitors;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;

namespace StreamChat.Tests.LowLevelClient
{
    internal class StreamChatLowLevelClientEventDrainTests
    {
        [SetUp]
        public void Up()
        {
            _authCredentials = new AuthCredentials("api123", "token123", "user123");
            _mockWebsocketClient = Substitute.For<IWebsocketClient>();
            _mockHttpClient = Substitute.For<IHttpClient>();
            _mockTimeService = Substitute.For<ITimeService>();
            _mockNetworkMonitor = Substitute.For<INetworkMonitor>();
            _mockApplicationInfo = Substitute.For<IApplicationInfo>();
            _mockLogs = new UnityLogs();
            _mockStreamClientConfig = Substitute.For<IStreamClientConfig>();

            _mockWebsocketClient.ConnectAsync(Arg.Any<Uri>()).Returns(Task.CompletedTask);
            _mockHttpClient
                .SendHttpRequestAsync(Arg.Is(HttpMethodType.Post), Arg.Any<Uri>(), Arg.Any<object>())
                .Returns(new HttpResponse(true, 200, "{\"events\":[]}", null, null));
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
        public void when_live_backlog_exceeds_count_cap_expect_one_update_handles_only_the_cap()
        {
            var client = CreateConnectedClient();
            client.EventDrainCountCap = 2;
            client.EventDrainTimeBudgetMs = 10_000;

            var received = new List<string>();
            client.MessageReceived += e => received.Add(e.Message.Id);

            EnqueueLiveMessages(Message("m1"), Message("m2"), Message("m3"), Message("m4"), Message("m5"));
            client.Update(0.1f);

            Assert.AreEqual(new[] { "m1", "m2" }, received.ToArray());

            client.Update(0.1f);
            Assert.AreEqual(new[] { "m1", "m2", "m3", "m4" }, received.ToArray());

            client.Update(0.1f);
            Assert.AreEqual(new[] { "m1", "m2", "m3", "m4", "m5" }, received.ToArray());
        }

        [Test]
        public void when_one_live_message_per_update_expect_it_drains_fully()
        {
            var client = CreateConnectedClient();
            client.EventDrainCountCap = 2;

            var received = 0;
            client.MessageReceived += _ => received++;

            EnqueueLiveMessages(Message("m1"));
            client.Update(0.1f);

            Assert.AreEqual(1, received);

            client.Update(0.1f);
            Assert.AreEqual(1, received);
        }

        [Test]
        public void when_budget_spent_expect_live_health_check_still_handled()
        {
            var client = CreateConnectedClient();
            client.EventDrainCountCap = 1;
            client.EventDrainTimeBudgetMs = 10_000;

            var messages = new List<string>();
            var healthChecks = 0;
            client.MessageReceived += e => messages.Add(e.Message.Id);
            client.EventReceived += payload =>
            {
                if (payload.Contains("health.check"))
                {
                    healthChecks++;
                }
            };

            EnqueueLiveMessages(Message("m1"), HealthCheck(), Message("m2"));
            client.Update(0.1f);

            Assert.AreEqual(new[] { "m1" }, messages.ToArray());
            Assert.GreaterOrEqual(healthChecks, 1);

            client.Update(0.1f);
            Assert.AreEqual(new[] { "m1", "m2" }, messages.ToArray());
        }

        [Test]
        public void when_time_budget_exhausted_expect_remaining_live_events_deferred()
        {
            var client = CreateConnectedClient();
            var clock = new ManualElapsedStopwatch();
            client.EventDrainStopwatch = clock;
            client.EventDrainTimeBudgetMs = 3;
            client.EventDrainCountCap = 1000;

            var received = new List<string>();
            client.MessageReceived += e =>
            {
                received.Add(e.Message.Id);
                clock.ElapsedMilliseconds = 10;
            };

            EnqueueLiveMessages(Message("m1"), Message("m2"), Message("m3"));
            client.Update(0.1f);

            Assert.AreEqual(new[] { "m1" }, received.ToArray());

            clock.ElapsedMilliseconds = 0;
            client.Update(0.1f);
            Assert.AreEqual(new[] { "m1", "m2" }, received.ToArray());
        }

        [Test]
        public void when_history_and_live_queued_expect_history_processed_first()
        {
            var client = CreateConnectedClient();
            client.EventDrainCountCap = 10;
            client.EventDrainTimeBudgetMs = 10_000;

            var received = new List<string>();
            client.MessageReceived += e => received.Add(e.Message.Id);

            StubSyncEvents(MessageEventJson("h1"), MessageEventJson("h2"));
            SetDisconnectionWatermark(client, new DateTimeOffset(2026, 8, 10, 11, 0, 0, TimeSpan.Zero));
            _mockTimeService.Now.Returns(new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero));

            EnqueueLiveMessages(Message("l1"), Message("l2"));

            var fetch = client.FetchAndProcessEventsSinceLastReceivedEvent(new[] { "messaging:test" });
            client.Update(0.1f);

            Assert.AreEqual(new[] { "h1", "h2", "l1", "l2" }, received.ToArray());
            Assert.IsTrue(fetch.IsCompleted);
            fetch.GetAwaiter().GetResult();
        }

        [Test]
        public void when_sync_returns_many_events_expect_they_span_updates_and_task_waits()
        {
            var client = CreateConnectedClient();
            client.EventDrainCountCap = 2;
            client.EventDrainTimeBudgetMs = 10_000;

            var received = new List<string>();
            client.MessageReceived += e => received.Add(e.Message.Id);

            StubSyncEvents(MessageEventJson("h1"), MessageEventJson("h2"), MessageEventJson("h3"),
                MessageEventJson("h4"), MessageEventJson("h5"));
            SetDisconnectionWatermark(client, new DateTimeOffset(2026, 8, 10, 11, 0, 0, TimeSpan.Zero));
            _mockTimeService.Now.Returns(new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero));

            var fetch = client.FetchAndProcessEventsSinceLastReceivedEvent(new[] { "messaging:test" });
            Assert.IsFalse(fetch.IsCompleted);

            client.Update(0.1f);
            Assert.AreEqual(new[] { "h1", "h2" }, received.ToArray());
            Assert.IsFalse(fetch.IsCompleted);

            client.Update(0.1f);
            Assert.AreEqual(new[] { "h1", "h2", "h3", "h4" }, received.ToArray());

            client.Update(0.1f);
            Assert.AreEqual(new[] { "h1", "h2", "h3", "h4", "h5" }, received.ToArray());
            Assert.IsTrue(fetch.IsCompleted);
            fetch.GetAwaiter().GetResult();
        }

        [Test]
        public void when_disconnect_mid_history_drain_expect_pending_cleared_and_no_double_apply()
        {
            var client = CreateConnectedClient();
            client.EventDrainCountCap = 1;
            client.EventDrainTimeBudgetMs = 10_000;
            SetupDisconnectRaisesDisconnected();

            var received = new List<string>();
            client.MessageReceived += e => received.Add(e.Message.Id);

            StubSyncEvents(MessageEventJson("h1"), MessageEventJson("h2"), MessageEventJson("h3"));
            SetDisconnectionWatermark(client, new DateTimeOffset(2026, 8, 10, 11, 0, 0, TimeSpan.Zero));
            _mockTimeService.Now.Returns(new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero));

            var fetch = client.FetchAndProcessEventsSinceLastReceivedEvent(new[] { "messaging:test" });
            client.Update(0.1f);
            Assert.AreEqual(new[] { "h1" }, received.ToArray());

            client.DisconnectAsync(DisconnectCause.ConnectionReleased).GetAwaiter().GetResult();
            Assert.IsTrue(fetch.IsCompleted);

            client.Update(0.1f);
            Assert.AreEqual(new[] { "h1" }, received.ToArray());
        }

        private readonly List<IDisposable> _resourcesToDispose = new List<IDisposable>();

        private AuthCredentials _authCredentials;
        private IWebsocketClient _mockWebsocketClient;
        private IApplicationInfo _mockApplicationInfo;
        private ILogs _mockLogs;
        private ITimeService _mockTimeService;
        private INetworkMonitor _mockNetworkMonitor;
        private IHttpClient _mockHttpClient;
        private IStreamClientConfig _mockStreamClientConfig;

        private StreamChatLowLevelClient CreateConnectedClient()
        {
            var client = new StreamChatLowLevelClient(_authCredentials, _mockWebsocketClient, _mockHttpClient,
                new NewtonsoftJsonSerializer(), _mockTimeService, _mockNetworkMonitor, _mockApplicationInfo,
                _mockLogs, _mockStreamClientConfig);
            _resourcesToDispose.Add(client);

            EnqueueLiveMessages(HealthCheck());
            client.Connect();
            client.Update(0.2f);
            Assert.AreEqual(ConnectionState.Connected, client.ConnectionState);
            return client;
        }

        private void EnqueueLiveMessages(params string[] messages)
        {
            var queue = new Queue<string>(messages);
            _mockWebsocketClient.TryDequeueMessage(out Arg.Any<string>()).Returns(arg =>
            {
                if (queue.Count == 0)
                {
                    return false;
                }

                arg[0] = queue.Dequeue();
                return true;
            });
        }

        private void StubSyncEvents(params string[] eventJsonObjects)
        {
            var body = "{\"events\":[" + string.Join(",", eventJsonObjects) + "]}";
            _mockHttpClient
                .SendHttpRequestAsync(Arg.Is(HttpMethodType.Post), Arg.Any<Uri>(), Arg.Any<object>())
                .Returns(new HttpResponse(true, 200, body, null, null));
        }

        private void SetupDisconnectRaisesDisconnected()
        {
            _mockWebsocketClient.When(_ => _.DisconnectAsync(Arg.Any<WebSocketCloseStatus>(), Arg.Any<string>()))
                .Do(_ => { _mockWebsocketClient.Disconnected += Raise.Event<Action>(); });
        }

        private static void SetDisconnectionWatermark(StreamChatLowLevelClient client, DateTimeOffset value)
        {
            var field = typeof(StreamChatLowLevelClient).GetField("_disconnectionLastEventReceivedAt",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.IsNotNull(field);
            field.SetValue(client, (DateTimeOffset?)value);
        }

        private static string Message(string id)
            => $"{{\"type\":\"message.new\",\"cid\":\"messaging:test\",\"message\":{{\"id\":\"{id}\",\"text\":\"{id}\",\"type\":\"regular\"}}}}";

        private static string MessageEventJson(string id) => Message(id);

        private static string HealthCheck() => "{\"connection_id\":\"fakeId\",\"type\":\"health.check\"}";

        private sealed class ManualElapsedStopwatch : IElapsedStopwatch
        {
            public double ElapsedMilliseconds { get; set; }

            public void Restart() => ElapsedMilliseconds = 0;
        }
    }
}
#endif
