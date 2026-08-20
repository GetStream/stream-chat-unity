#if STREAM_TESTS_ENABLED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute;
using NUnit.Framework;
using StreamChat.Core.Configs;
using StreamChat.Core.LowLevelClient;
using StreamChat.Libs.AppInfo;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.NetworkMonitors;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;

namespace StreamChat.Tests.StateSync.Unit
{
    /// <summary>
    /// Unit tests for the two history application modes on <see cref="StreamChatLowLevelClient"/> -
    /// <see cref="StateRecoveryStrategy.ReplayEvents"/> and
    /// <see cref="StateRecoveryStrategy.BatchStateUpdate"/> - and for the <c>/sync</c> request shape.
    /// </summary>
    internal class StateRecoveryLowLevelTests
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

            _lowLevelClient = CreateClient();
            _lowLevelClient.Update(0.1f);

            _mockHttpClient
                .SendHttpRequestAsync(Arg.Is(HttpMethodType.Post), Arg.Any<Uri>(), Arg.Any<object>())
                .Returns(new HttpResponse(true, 200, "{\"events\":[]}", null, null));
        }

        [TearDown]
        public void TearDown()
        {
            for (var i = _clientsToDispose.Count - 1; i >= 0; i--)
            {
                _clientsToDispose[i].Dispose();
            }

            _clientsToDispose.Clear();
            _lowLevelClient = null;
        }

        [Test]
        public void when_sync_requested_with_more_than_100_cids_expect_only_100_sent()
        {
            var now = new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);
            _mockTimeService.Now.Returns(now);
            SetDisconnectionLastEventReceivedAt(_lowLevelClient, now.AddHours(-1));

            var cids = Enumerable.Range(0, 150).Select(i => $"messaging:channel-{i}").ToList();

            _lowLevelClient.TrySyncHistoryAsync(cids).GetAwaiter().GetResult();

            _mockHttpClient.Received(1).SendHttpRequestAsync(
                Arg.Is(HttpMethodType.Post),
                Arg.Is<Uri>(uri => uri.AbsolutePath.EndsWith("/sync")),
                Arg.Is<object>(body => CountSyncCids(body) == StreamChatLowLevelClient.MaxSyncChannelCids));
        }

        [Test]
        public void when_sync_requested_expect_inaccessible_cids_asked_for()
        {
            var now = new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);
            _mockTimeService.Now.Returns(now);
            SetDisconnectionLastEventReceivedAt(_lowLevelClient, now.AddHours(-1));

            _lowLevelClient.TrySyncHistoryAsync(new[] { "messaging:a" }).GetAwaiter().GetResult();

            // Without this the response cannot distinguish a deleted channel from one the query
            // happened to omit, and recovery would keep retrying it forever.
            _mockHttpClient.Received(1).SendHttpRequestAsync(
                Arg.Is(HttpMethodType.Post),
                Arg.Is<Uri>(uri => uri.AbsolutePath.EndsWith("/sync")),
                Arg.Is<object>(body => GetBoolMember(body, "WithInaccessibleCids") == true));
        }

        [Test]
        public void when_history_batch_applied_expect_no_public_message_received()
        {
            var received = 0;
            _lowLevelClient.MessageReceived += _ => received++;

            var result = _lowLevelClient.ApplyHistoryEvents(new List<object> { MessageNewJson("msg-1", NewestCreatedAt) });

            Assert.AreEqual(0, received, "A silent history batch must not raise public per-event callbacks.");
            Assert.AreEqual(0, result.FailedEventCount);
            Assert.AreEqual(NewestCreatedAt, result.MaxAppliedCreatedAt);
        }

        [Test]
        public void when_history_batch_replayed_expect_public_message_received()
        {
            var received = 0;
            _lowLevelClient.MessageReceived += _ => received++;

            _lowLevelClient.ReplayHistoryEvents(new List<object> { MessageNewJson("msg-1", NewestCreatedAt) });

            Assert.AreEqual(1, received,
                "ReplayEvents is the default strategy and must keep raising per-event callbacks for back-compat.");
        }

        [Test]
        public void when_history_batch_contains_custom_event_expect_it_delivered_per_event()
        {
            var received = new List<string>();
            _lowLevelClient.CustomEventReceived += e => received.Add(e.Type);

            _lowLevelClient.ApplyHistoryEvents(new List<object>
            {
                CustomEventJson("game.state", NewestCreatedAt),
            });

            // Custom events have no representation in local state, so suppressing them would lose the
            // payload with no way for a consumer to recover it.
            Assert.AreEqual(new[] { "game.state" }, received.ToArray());
        }

        [Test]
        public void when_history_batch_applied_expect_watermark_advanced_once_to_newest_applied_event()
        {
            var client = CreateClient();

            client.ApplyHistoryEvents(new List<object>
            {
                MessageNewJson("msg-1", NewestCreatedAt.AddMinutes(-10)),
                MessageNewJson("msg-2", NewestCreatedAt),
                MessageNewJson("msg-3", NewestCreatedAt.AddMinutes(-5)),
            });

            Assert.AreEqual(NewestCreatedAt, GetLastEventReceivedAt(client),
                "The batch must advance the watermark exactly once, to its newest event.");
        }

        [Test]
        public void when_history_batch_contains_malformed_event_expect_remaining_events_still_applied()
        {
            var client = CreateClient();
            var newest = NewestCreatedAt;

            var result = client.ApplyHistoryEvents(new List<object>
            {
                MessageNewJson("msg-1", newest.AddMinutes(-10)),
                $"{{\"type\":\"message.new\",\"cid\":\"messaging:test\",\"created_at\":\"{newest.AddMinutes(-1):O}\",\"message\":\"not-an-object\"}}",
                MessageNewJson("msg-3", newest.AddMinutes(-5)),
            });

            Assert.AreEqual(1, result.FailedEventCount);

            // The watermark must not claim the failed event was applied, or the next reconnect would
            // never ask for it again.
            Assert.AreEqual(newest.AddMinutes(-5), result.MaxAppliedCreatedAt);
            Assert.AreEqual(newest.AddMinutes(-5), GetLastEventReceivedAt(client));
        }

        [Test]
        public void when_history_event_older_than_watermark_expect_watermark_not_regressed()
        {
            var client = CreateClient();
            SetLastEventReceivedAt(client, NewestCreatedAt);

            client.ApplyHistoryEvents(new List<object> { MessageNewJson("msg-1", NewestCreatedAt.AddDays(-1)) });

            Assert.AreEqual(NewestCreatedAt, GetLastEventReceivedAt(client));
        }

        [Test]
        public void when_health_check_arrives_on_live_socket_expect_liveness_stamped_before_handlers()
        {
            var client = CreateClientWithMessages(HealthCheckJson());
            client.Connect();
            client.Update(0.2f);

            _mockTimeService.Time.Returns(12f);
            EnqueueMessages(HealthCheckJson());
            client.Update(0.2f);

            Assert.AreEqual(12f, GetLastHealthCheckReceivedTime(client),
                "Liveness must be stamped when the health check is read, not after consumer handlers run.");
        }

        [Test]
        public void when_health_check_arrives_from_history_replay_expect_liveness_not_stamped()
        {
            var client = CreateClientWithMessages(HealthCheckJson());
            client.Connect();
            client.Update(0.2f);

            var stampedOnConnect = GetLastHealthCheckReceivedTime(client);

            _mockTimeService.Time.Returns(99f);
            client.ReplayHistoryEvents(new List<object> { HealthCheckJson() });

            Assert.AreEqual(stampedOnConnect, GetLastHealthCheckReceivedTime(client),
                "A replayed health check proves nothing about the current socket and must not extend liveness.");
        }

        private static readonly DateTimeOffset NewestCreatedAt =
            new DateTimeOffset(2026, 8, 10, 11, 0, 0, TimeSpan.Zero);

        private const string TestCid = "messaging:test";

        private static string MessageNewJson(string messageId, DateTimeOffset createdAt)
            => $"{{\"type\":\"message.new\",\"cid\":\"{TestCid}\",\"created_at\":\"{createdAt:O}\"," +
               $"\"message\":{{\"id\":\"{messageId}\",\"text\":\"hi\",\"created_at\":\"{createdAt:O}\"," +
               $"\"updated_at\":\"{createdAt:O}\",\"user\":{{\"id\":\"user-1\"}}}}}}";

        private static string CustomEventJson(string type, DateTimeOffset createdAt)
            => $"{{\"type\":\"{type}\",\"cid\":\"{TestCid}\",\"created_at\":\"{createdAt:O}\"," +
               "\"user\":{\"id\":\"user-1\"}}";

        private static string HealthCheckJson()
            => "{\"connection_id\":\"fakeId\",\"type\":\"health.check\"}";

        private static int CountSyncCids(object requestBody)
        {
            var list = GetMember(requestBody, "ChannelCids") as System.Collections.IList;
            return list?.Count ?? -1;
        }

        private static bool? GetBoolMember(object requestBody, string name) => GetMember(requestBody, name) as bool?;

        private static object GetMember(object requestBody, string name)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var property = requestBody.GetType().GetProperty(name, flags);
            if (property != null)
            {
                return property.GetValue(requestBody);
            }

            return requestBody.GetType().GetField(name, flags)?.GetValue(requestBody);
        }

        private static void SetDisconnectionLastEventReceivedAt(StreamChatLowLevelClient client, DateTimeOffset value)
            => GetPrivateField("_disconnectionLastEventReceivedAt").SetValue(client, (DateTimeOffset?)value);

        private static void SetLastEventReceivedAt(StreamChatLowLevelClient client, DateTimeOffset value)
            => GetPrivateField("_lastEventReceivedAt").SetValue(client, (DateTimeOffset?)value);

        private static DateTimeOffset? GetLastEventReceivedAt(StreamChatLowLevelClient client)
            => (DateTimeOffset?)GetPrivateField("_lastEventReceivedAt").GetValue(client);

        private static float GetLastHealthCheckReceivedTime(StreamChatLowLevelClient client)
            => (float)GetPrivateField("_lastHealthCheckReceivedTime").GetValue(client);

        private static FieldInfo GetPrivateField(string name)
        {
            var field = typeof(StreamChatLowLevelClient).GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, $"Expected {name} field to exist.");
            return field;
        }

        private StreamChatLowLevelClient CreateClient()
        {
            var client = new StreamChatLowLevelClient(_authCredentials, _mockWebsocketClient, _mockHttpClient,
                _serializer, _mockTimeService, _mockNetworkMonitor, _mockApplicationInfo, _mockLogs,
                _mockStreamClientConfig);

            _clientsToDispose.Add(client);
            return client;
        }

        private StreamChatLowLevelClient CreateClientWithMessages(params string[] websocketMessages)
        {
            var client = CreateClient();
            _mockWebsocketClient.ConnectAsync(Arg.Any<Uri>()).Returns(System.Threading.Tasks.Task.CompletedTask);

            _mockWebsocketClient.TryDequeueMessage(out Arg.Any<string>()).Returns(arg =>
            {
                if (_pendingWebsocketMessages.Count == 0)
                {
                    return false;
                }

                arg[0] = _pendingWebsocketMessages.Dequeue();
                return true;
            });

            EnqueueMessages(websocketMessages);
            return client;
        }

        private void EnqueueMessages(params string[] websocketMessages)
        {
            foreach (var message in websocketMessages)
            {
                _pendingWebsocketMessages.Enqueue(message);
            }
        }

        private readonly List<StreamChatLowLevelClient> _clientsToDispose = new List<StreamChatLowLevelClient>();
        private readonly Queue<string> _pendingWebsocketMessages = new Queue<string>();

        private StreamChatLowLevelClient _lowLevelClient;
        private AuthCredentials _authCredentials;
        private IWebsocketClient _mockWebsocketClient;
        private IApplicationInfo _mockApplicationInfo;
        private ILogs _mockLogs;
        private ISerializer _serializer;
        private ITimeService _mockTimeService;
        private INetworkMonitor _mockNetworkMonitor;
        private IHttpClient _mockHttpClient;
        private IStreamClientConfig _mockStreamClientConfig;
    }
}
#endif
