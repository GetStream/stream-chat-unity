#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using NSubstitute;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Configs;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.Responses;
using StreamChat.Core.State.Caches;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.AppInfo;
using StreamChat.Libs.Auth;
using StreamChat.Libs.ChatInstanceRunner;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.NetworkMonitors;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;

namespace StreamChat.Tests.StateSync.Unit
{
    /// <summary>
    /// Unit tests for reconnect state recovery on <see cref="StreamChatClient"/>, driven entirely
    /// through mocked transports so connection state transitions can be sequenced exactly.
    /// </summary>
    internal class StateRecoveryClientTests
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
            _config = new StreamClientConfig();

            _mockWebsocketClient.ConnectAsync(Arg.Any<Uri>()).Returns(Task.CompletedTask);
            _mockWebsocketClient.DisconnectAsync(Arg.Any<System.Net.WebSockets.WebSocketCloseStatus>(),
                Arg.Any<string>()).Returns(Task.CompletedTask);

            _mockWebsocketClient.TryDequeueMessage(out Arg.Any<string>()).Returns(arg =>
            {
                if (_pendingWebsocketMessages.Count == 0)
                {
                    return false;
                }

                arg[0] = _pendingWebsocketMessages.Dequeue();
                return true;
            });

            RespondWith(SyncEndpoint, "{\"events\":[]}");
            RespondWith(QueryChannelsEndpoint, "{\"channels\":[]}");

            _client = (StreamChatClient)StreamChatClient.CreateClientWithCustomDependencies(_mockWebsocketClient,
                _mockHttpClient, new NewtonsoftJsonSerializer(), _mockTimeService, _mockNetworkMonitor,
                _mockApplicationInfo, _mockLogs, _config);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _client = null;
            _pendingWebsocketMessages.Clear();
            _recoveredEvents.Clear();
        }

        [Test]
        public void when_first_connect_expect_no_recovery_and_no_state_recovered_event()
        {
            Connect();

            Assert.AreEqual(0, _recoveredEvents.Count,
                "A fresh login is not a recovery - firing StateRecovered here would make it useless as a signal.");
            AssertQueryChannelsCallCount(0);
        }

        [Test]
        public void when_reconnected_expect_channels_requeried_even_though_sync_was_skipped()
        {
            Connect();
            WatchChannel("messaging:a");
            WatchChannel("messaging:b");

            DropConnection();
            Reconnect();

            // The whole point of #227/#232: the re-query is what re-establishes the watches, so it has
            // to run whether or not the /sync catch-up did anything. Here it was skipped outright,
            // because the health check carried no created_at so there is no sync point.
            AssertQueryChannelsCallCount(1);
            Assert.AreEqual(1, _recoveredEvents.Count);
        }

        [Test]
        public void when_connection_drops_expect_watches_released_and_snapshot_taken()
        {
            Connect();
            var channel = WatchChannel("messaging:a");

            DropConnection();

            // The server dropped the watch, so continuing to report it would be a lie - and would make
            // IStreamChannel.WatchAsync a silent no-op for a channel that is not actually watched.
            Assert.IsFalse(channel.IsWatched);
            Assert.AreEqual(0, _client.WatchedChannels.Count);
            Assert.AreEqual(new[] { "messaging:a" }, RecoverySnapshot());
        }

        [Test]
        public void when_reconnect_attempt_fails_expect_recovery_snapshot_preserved()
        {
            Connect();
            WatchChannel("messaging:a");
            WatchChannel("messaging:b");

            DropConnection();
            FailReconnectAttempt();
            FailReconnectAttempt();

            // A failed attempt transitions Connecting -> Disconnected with an already-empty watch list.
            // Re-snapshotting there would discard the only record of what needs recovering, and the
            // attempt that eventually succeeds would restore nothing - the flaky-mobile-network case.
            Assert.AreEqual(new[] { "messaging:a", "messaging:b" }, RecoverySnapshot().OrderBy(_ => _).ToArray());

            Reconnect();
            AssertQueryChannelsCallCount(1);
        }

        [Test]
        public void when_channels_cannot_be_recovered_expect_them_reported_as_unrecovered()
        {
            Connect();
            WatchChannel("messaging:a");

            DropConnection();
            Reconnect();

            // The mocked query returns no channels, which is what the server does for a channel that
            // was deleted or that the local user lost access to while offline.
            Assert.AreEqual(1, _recoveredEvents.Count);
            Assert.AreEqual(0, _recoveredEvents[0].Channels.Count);
            Assert.AreEqual(new[] { "messaging:a" }, _recoveredEvents[0].UnrecoveredChannelCids.ToArray());
            Assert.IsFalse(_recoveredEvents[0].IsComplete);
        }

        [Test]
        public void when_recovery_query_fails_expect_remaining_chunks_still_queried()
        {
            Connect();
            for (var i = 0; i < 40; i++)
            {
                WatchChannel($"messaging:channel-{i:D2}");
            }

            var callCount = 0;
            _mockHttpClient
                .SendHttpRequestAsync(Arg.Is(HttpMethodType.Post),
                    Arg.Is<Uri>(uri => uri.AbsolutePath.EndsWith(QueryChannelsEndpoint)), Arg.Any<object>())
                .Returns(_ =>
                {
                    callCount++;
                    return callCount == 1
                        ? new HttpResponse(false, 429, "{\"code\":9,\"message\":\"rate limited\"}", null, null)
                        : new HttpResponse(true, 200, "{\"channels\":[]}", null, null);
                });

            DropConnection();
            Reconnect();

            // 40 cids is two chunks of 30 and 10. There is no later retry within a connection, so a
            // failed chunk must not cost the remaining chunks their recovery.
            Assert.AreEqual(2, callCount);
            Assert.AreEqual(1, _recoveredEvents.Count);
        }

        [Test]
        public void when_recovery_set_exceeds_cap_expect_only_capped_channels_queried()
        {
            Connect();
            for (var i = 0; i < StreamChatClient.MaxRecoveredChannels + 25; i++)
            {
                WatchChannel($"messaging:channel-{i:D3}");
            }

            DropConnection();
            Reconnect();

            // Capped at 100, chunked by 30 -> 4 requests. Uncapped this would be 5, and would keep
            // growing with the watch list on every single reconnect.
            AssertQueryChannelsCallCount(4);
        }

        [Test]
        public void when_strategy_is_disabled_expect_no_recovery_and_watches_left_untouched()
        {
            _config.StateRecoveryStrategy = StateRecoveryStrategy.Disabled;

            Connect();
            var channel = WatchChannel("messaging:a");

            DropConnection();
            Reconnect();

            AssertQueryChannelsCallCount(0);
            Assert.AreEqual(0, _recoveredEvents.Count);

            // Disabled means the SDK does nothing, so WatchedChannels stays the record of what the
            // consumer was watching and is theirs to recover from.
            Assert.IsTrue(channel.IsWatched);
            Assert.AreEqual(1, _client.WatchedChannels.Count);
        }

        [Test]
        public void when_user_disconnects_and_connects_again_expect_no_recovery_of_previous_session()
        {
            Connect();
            WatchChannel("messaging:a");

            _client.DisconnectUserAsync().GetAwaiter().GetResult();
            DropConnection();

            Connect();

            // A new login must not recover, or re-watch, channels belonging to the session that ended -
            // possibly for a different user.
            AssertQueryChannelsCallCount(0);
            Assert.AreEqual(0, _recoveredEvents.Count);
        }

        [Test]
        public void when_channel_unwatched_while_disconnected_expect_it_not_recovered()
        {
            Connect();
            var channel = WatchChannel("messaging:a");
            WatchChannel("messaging:b");

            DropConnection();
            InvokeMarkChannelUnwatched(channel);
            Reconnect();

            Assert.AreEqual(new[] { "messaging:b" }, RecoverySnapshot());
        }

        [Test]
        public void when_recovery_query_returns_channel_expect_watch_restored_and_state_recovered()
        {
            Connect();
            var channel = WatchChannel("messaging:a");

            RespondWith(QueryChannelsEndpoint, QueryChannelsJson("messaging:a", "restored"));

            DropConnection();
            Reconnect();

            Assert.IsTrue(channel.IsWatched);
            Assert.AreEqual(1, _client.WatchedChannels.Count);
            Assert.AreSame(channel, _client.WatchedChannels.Single());
            Assert.AreEqual("restored", channel.Name);
            Assert.AreEqual(1, _recoveredEvents.Count);
            Assert.AreSame(channel, _recoveredEvents[0].Channels.Single());
            Assert.IsTrue(_recoveredEvents[0].IsComplete);
        }

        [UnityTest]
        public IEnumerator when_stale_recovery_query_completes_after_newer_recovery_expect_stale_result_not_applied()
        {
            Connect();
            var channel = WatchChannel("messaging:a");

            var staleQuery = new TaskCompletionSource<HttpResponse>();
            var currentQuery = new TaskCompletionSource<HttpResponse>();
            HoldQueryChannelsResponses(staleQuery.Task, currentQuery.Task);

            DropConnection();
            Reconnect();

            Assert.AreEqual(0, _recoveredEvents.Count,
                "The first recovery must still be waiting on its query.");
            Assert.IsFalse(channel.IsWatched);
            Assert.AreEqual(0, _client.WatchedChannels.Count);

            // Increments generation. Snapshot is kept because watches were already cleared.
            DropConnection();
            Reconnect();

            Assert.AreEqual(0, _recoveredEvents.Count);

            currentQuery.SetResult(QueryChannelsHttpResponse("messaging:a", "fresh-B"));
            yield return WaitUntil(() => channel.Name == "fresh-B",
                "Current-generation recovery did not apply the fresh query payload.");

            Assert.IsTrue(channel.IsWatched);
            Assert.AreEqual(1, _client.WatchedChannels.Count);
            Assert.AreSame(channel, _client.WatchedChannels.Single());
            Assert.AreEqual(1, _recoveredEvents.Count);
            Assert.AreSame(channel, _recoveredEvents[0].Channels.Single());
            Assert.IsTrue(_recoveredEvents[0].IsComplete);

            staleQuery.SetResult(QueryChannelsHttpResponse("messaging:a", "stale-A"));
            // Drain posted continuations so a missing generation gate would have applied by now.
            for (var i = 0; i < 5; i++)
            {
                Update();
                yield return null;
            }

            Assert.AreEqual("fresh-B", channel.Name,
                "A late query from a superseded recovery must not replace channel state.");
            Assert.IsTrue(channel.IsWatched);
            Assert.AreEqual(1, _client.WatchedChannels.Count);
            Assert.AreEqual(1, _recoveredEvents.Count,
                "The stale recovery must not raise StateRecovered after a newer one already did.");
        }

        private const string SyncEndpoint = "/sync";
        private const string QueryChannelsEndpoint = "/channels";

        private void RespondWith(string endpointSuffix, string json)
        {
            _mockHttpClient
                .SendHttpRequestAsync(Arg.Is(HttpMethodType.Post),
                    Arg.Is<Uri>(uri => uri.AbsolutePath.EndsWith(endpointSuffix)), Arg.Any<object>())
                .Returns(new HttpResponse(true, 200, json, null, null));
        }

        private void HoldQueryChannelsResponses(params Task<HttpResponse>[] responses)
        {
            var queue = new Queue<Task<HttpResponse>>(responses);
            _mockHttpClient
                .SendHttpRequestAsync(Arg.Is(HttpMethodType.Post),
                    Arg.Is<Uri>(uri => uri.AbsolutePath.EndsWith(QueryChannelsEndpoint)), Arg.Any<object>())
                .Returns(_ => queue.Dequeue());
        }

        private static HttpResponse QueryChannelsHttpResponse(string cid, string name)
            => new HttpResponse(true, 200, QueryChannelsJson(cid, name), null, null);

        private static string QueryChannelsJson(string cid, string name)
        {
            var separatorIndex = cid.IndexOf(':');
            var type = cid.Substring(0, separatorIndex);
            var id = cid.Substring(separatorIndex + 1);
            return "{\"channels\":[{\"channel\":{" +
                   $"\"cid\":\"{cid}\",\"id\":\"{id}\",\"type\":\"{type}\",\"name\":\"{name}\"" +
                   "},\"messages\":[],\"members\":[]}]}";
        }

        private IEnumerator WaitUntil(Func<bool> condition, string message, int maxFrames = 30)
        {
            var frames = 0;
            while (!condition() && frames < maxFrames)
            {
                Update();
                yield return null;
                frames++;
            }

            Assert.IsTrue(condition(), message);
        }

        private void AssertQueryChannelsCallCount(int expected)
        {
            _mockHttpClient.Received(expected).SendHttpRequestAsync(Arg.Is(HttpMethodType.Post),
                Arg.Is<Uri>(uri => uri.AbsolutePath.EndsWith(QueryChannelsEndpoint)), Arg.Any<object>());
        }

        private void Connect()
        {
            _client.StateRecovered -= OnStateRecovered;
            _client.StateRecovered += OnStateRecovered;

            var connectTask = _client.ConnectUserAsync(_authCredentials);
            _pendingWebsocketMessages.Enqueue(HealthCheckJson);
            Update();

            Assert.IsTrue(connectTask.IsCompleted, "Expected the mocked health check to complete the connect.");
            Assert.AreEqual(ConnectionState.Connected, _client.ConnectionState);
        }

        private void Reconnect()
        {
            _client.InternalLowLevelClient.Connect();
            _pendingWebsocketMessages.Enqueue(HealthCheckJson);
            Update();

            Assert.AreEqual(ConnectionState.Connected, _client.ConnectionState);
        }

        private void DropConnection()
        {
            _mockWebsocketClient.Disconnected += Raise.Event<Action>();
            Update();

            Assert.AreEqual(ConnectionState.Disconnected, _client.ConnectionState);
        }

        private void FailReconnectAttempt()
        {
            _client.InternalLowLevelClient.Connect();
            Assert.AreEqual(ConnectionState.Connecting, _client.ConnectionState);

            _mockWebsocketClient.ConnectionFailed += Raise.Event<Action>();
            Update();

            Assert.AreEqual(ConnectionState.Disconnected, _client.ConnectionState);
        }

        private void Update() => ((IStreamChatClientEventsListener)_client).Update();

        private void OnStateRecovered(StreamStateRecoveredEventArgs args) => _recoveredEvents.Add(args);

        private IStreamChannel WatchChannel(string cid)
        {
            var separatorIndex = cid.IndexOf(':');
            var channel = _client.InternalCache.TryCreateOrUpdate(new ChannelResponseInternalDTO
            {
                Cid = cid,
                Type = cid.Substring(0, separatorIndex),
                Id = cid.Substring(separatorIndex + 1),
            });

            InvokePrivate("MarkChannelWatched", channel);
            return channel;
        }

        private void InvokeMarkChannelUnwatched(IStreamChannel channel)
            => InvokePrivate("InternalMarkChannelUnwatched", channel);

        private void InvokePrivate(string methodName, object argument)
        {
            var method = typeof(StreamChatClient).GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(method, $"Expected {methodName} to exist.");
            method.Invoke(_client, new[] { argument });
        }

        private string[] RecoverySnapshot()
        {
            var field = typeof(StreamChatClient).GetField("_recoveryChannelCids",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, "Expected _recoveryChannelCids to exist.");
            return ((List<string>)field.GetValue(_client)).ToArray();
        }

        private const string HealthCheckJson = "{\"connection_id\":\"fakeId\",\"type\":\"health.check\"}";

        private readonly Queue<string> _pendingWebsocketMessages = new Queue<string>();
        private readonly List<StreamStateRecoveredEventArgs> _recoveredEvents =
            new List<StreamStateRecoveredEventArgs>();

        private StreamChatClient _client;
        private StreamClientConfig _config;
        private AuthCredentials _authCredentials;
        private IWebsocketClient _mockWebsocketClient;
        private IApplicationInfo _mockApplicationInfo;
        private ILogs _mockLogs;
        private ITimeService _mockTimeService;
        private INetworkMonitor _mockNetworkMonitor;
        private IHttpClient _mockHttpClient;
    }
}
#endif
