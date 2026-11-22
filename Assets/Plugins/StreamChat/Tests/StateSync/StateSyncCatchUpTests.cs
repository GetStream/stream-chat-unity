#if STREAM_TESTS_ENABLED
using System;
using System.Reflection;
using System.Threading.Tasks;
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
    /// Unit tests for post-disconnect /sync catch-up in <see cref="StreamChatLowLevelClient"/>.
    /// </summary>
    internal class StateSyncCatchUpTests
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
            _mockLogs = new UnityLogs();
            _mockStreamClientConfig = Substitute.For<IStreamClientConfig>();

            _lowLevelClient = new StreamChatLowLevelClient(_authCredentials, _mockWebsocketClient, _mockHttpClient,
                _serializer, _mockTimeService, _mockNetworkMonitor, _mockApplicationInfo, _mockLogs,
                _mockStreamClientConfig);
            _lowLevelClient.Update(0.1f);

            _mockHttpClient
                .SendHttpRequestAsync(Arg.Is(HttpMethodType.Post), Arg.Any<Uri>(), Arg.Any<object>())
                .Returns(new HttpResponse(true, 200, "{\"events\":[]}", null, null));
        }

        [TearDown]
        public void TearDown()
        {
            _lowLevelClient.Dispose();
            _lowLevelClient = null;

            _mockWebsocketClient = null;
            _mockHttpClient = null;
            _serializer = null;
            _mockTimeService = null;
            _mockLogs = null;
            _mockStreamClientConfig = null;
        }

        [Test]
        public void when_last_event_older_than_30_days_expect_sync_not_called()
        {
            var now = new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);
            _mockTimeService.Now.Returns(now);
            SetDisconnectionLastEventReceivedAt(now.AddDays(-31));

            _lowLevelClient.FetchAndProcessEventsSinceLastReceivedEvent(new[] { TestChannelCid }).GetAwaiter()
                .GetResult();

            AssertSyncNotCalled();
        }

        [TestCase(-1, TestName = "1 day ago")]
        [TestCase(-30, TestName = "30 days ago (boundary)")]
        public void when_last_event_within_30_days_expect_sync_called(int daysSinceLastEvent)
        {
            var now = new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);
            _mockTimeService.Now.Returns(now);
            SetDisconnectionLastEventReceivedAt(now.AddDays(daysSinceLastEvent));

            _lowLevelClient.FetchAndProcessEventsSinceLastReceivedEvent(new[] { TestChannelCid }).GetAwaiter()
                .GetResult();

            _mockHttpClient.Received(1).SendHttpRequestAsync(
                Arg.Is(HttpMethodType.Post),
                Arg.Is<Uri>(uri => uri.AbsolutePath.EndsWith("/sync")),
                Arg.Any<object>());
        }

        [Test]
        public void when_disconnection_timestamp_missing_expect_sync_not_called()
        {
            var now = new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);
            _mockTimeService.Now.Returns(now);

            _lowLevelClient.FetchAndProcessEventsSinceLastReceivedEvent(new[] { TestChannelCid }).GetAwaiter()
                .GetResult();

            AssertSyncNotCalled();
        }

        /// <summary>
        /// /sync replays historical events through the same handlers as live events. Because the catch-up runs
        /// asynchronously after reconnect, live events can already have advanced the watermark by the time the
        /// replay is processed. Rewinding it here would make the next disconnect sync from a stale point.
        /// </summary>
        [Test]
        public void when_sync_replays_events_older_than_watermark_expect_watermark_not_regressed()
        {
            var now = new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);
            _mockTimeService.Now.Returns(now);

            SetLastEventReceivedAt(now);
            SetDisconnectionLastEventReceivedAt(now.AddHours(-1));
            StubSyncResponseWithMessageEvent(now.AddHours(-1));

            FetchAndPumpUntilDone();

            Assert.AreEqual(now, GetLastEventReceivedAt());
        }

        [Test]
        public void when_sync_replays_events_newer_than_watermark_expect_watermark_advanced()
        {
            var now = new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);
            _mockTimeService.Now.Returns(now);

            var replayedEventCreatedAt = now.AddHours(-1);
            SetLastEventReceivedAt(now.AddHours(-2));
            SetDisconnectionLastEventReceivedAt(now.AddHours(-2));
            StubSyncResponseWithMessageEvent(replayedEventCreatedAt);

            FetchAndPumpUntilDone();

            Assert.AreEqual(replayedEventCreatedAt, GetLastEventReceivedAt());
        }

        private const string TestChannelCid = "messaging:test-channel";

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

        private void AssertSyncNotCalled()
            => _mockHttpClient.DidNotReceive().SendHttpRequestAsync(
                Arg.Is(HttpMethodType.Post),
                Arg.Is<Uri>(uri => uri.AbsolutePath.EndsWith("/sync")),
                Arg.Any<object>());

        private void StubSyncResponseWithMessageEvent(DateTimeOffset eventCreatedAt)
        {
            var body =
                $"{{\"events\":[{{\"type\":\"message.new\",\"cid\":\"{TestChannelCid}\",\"created_at\":\"{eventCreatedAt:O}\"}}]}}";

            _mockHttpClient
                .SendHttpRequestAsync(Arg.Is(HttpMethodType.Post), Arg.Any<Uri>(), Arg.Any<object>())
                .Returns(new HttpResponse(true, 200, body, null, null));
        }

        private void SetDisconnectionLastEventReceivedAt(DateTimeOffset value)
            => GetPrivateField("_disconnectionLastEventReceivedAt").SetValue(_lowLevelClient, (DateTimeOffset?)value);

        private void SetLastEventReceivedAt(DateTimeOffset value)
            => GetPrivateField("_lastEventReceivedAt").SetValue(_lowLevelClient, (DateTimeOffset?)value);

        private DateTimeOffset? GetLastEventReceivedAt()
            => (DateTimeOffset?)GetPrivateField("_lastEventReceivedAt").GetValue(_lowLevelClient);

        private void FetchAndPumpUntilDone()
        {
            var task = _lowLevelClient.FetchAndProcessEventsSinceLastReceivedEvent(new[] { TestChannelCid });
            var frames = 0;
            while (!task.IsCompleted && frames < 1000)
            {
                _lowLevelClient.Update(0.1f);
                frames++;
            }

            Assert.IsTrue(task.IsCompleted, "FetchAndProcessEventsSinceLastReceivedEvent did not finish after Update pumping.");
            task.GetAwaiter().GetResult();
        }

        private static FieldInfo GetPrivateField(string name)
        {
            var field = typeof(StreamChatLowLevelClient).GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, $"Expected {name} field to exist.");
            return field;
        }
    }
}
#endif
