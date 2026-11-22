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
using StreamChat.Core.InternalDTO.Models;
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

            // These tests sequence connection state transitions by hand. The default strategy
            // spends its first 5 attempts reconnecting instantly, and because ITimeService is
            // mocked to a constant time, a dropped connection would be picked up by the very
            // same Update() that processed the drop - leaving no observable Disconnected state.
            _client.InternalLowLevelClient.SetReconnectStrategySettings(ReconnectStrategy.Never,
                exponentialMinInterval: null, exponentialMaxInterval: null, constantInterval: null);
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

            // /sync is skipped here (no last_sync_at). Recovery must still re-query so the
            // new socket watches the same channels again.
            AssertQueryChannelsCallCount(1);
            Assert.AreEqual(1, _recoveredEvents.Count);
            _mockHttpClient.Received().SendHttpRequestAsync(
                Arg.Is(HttpMethodType.Post),
                Arg.Is<Uri>(uri => uri.AbsolutePath.EndsWith(QueryChannelsEndpoint)),
                Arg.Is<object>(body => RequestHasJsonBool(body, "watch", true)
                                       && RequestHasJsonBool(body, "state", true)));
        }

        [Test]
        public void when_connection_drops_expect_watches_released()
        {
            Connect();
            var channel = WatchChannel("messaging:a");

            DropConnection();

            Assert.IsFalse(channel.IsWatched);
            Assert.AreEqual(0, _client.WatchedChannels.Count);
        }

        [Test]
        public void when_reconnect_attempt_fails_expect_channels_still_recovered()
        {
            Connect();
            WatchChannel("messaging:a");
            WatchChannel("messaging:b");

            DropConnection();
            FailReconnectAttempt();
            FailReconnectAttempt();
            Reconnect();

            // Failed Connecting -> Disconnected must not forget the channels that were
            // watched before the outage. The attempt that succeeds still recovers them.
            AssertQueryChannelsCallCount(1);
            _mockHttpClient.Received().SendHttpRequestAsync(
                Arg.Is(HttpMethodType.Post),
                Arg.Is<Uri>(uri => uri.AbsolutePath.EndsWith(QueryChannelsEndpoint)),
                Arg.Is<object>(body => RequestBodyContains(body, "messaging:a")
                                       && RequestBodyContains(body, "messaging:b")));
        }

        [Test]
        public void when_channels_cannot_be_recovered_expect_them_reported_as_unrecovered()
        {
            Connect();
            WatchChannel("messaging:a");

            DropConnection();
            Reconnect();

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

            Assert.AreEqual(2, callCount);
            Assert.AreEqual(1, _recoveredEvents.Count);
            Assert.AreEqual(40, _recoveredEvents[0].UnrecoveredChannelCids.Count);
            Assert.IsFalse(_recoveredEvents[0].IsComplete);
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

            _mockHttpClient.Received(1).SendHttpRequestAsync(
                Arg.Is(HttpMethodType.Post),
                Arg.Is<Uri>(uri => uri.AbsolutePath.EndsWith(QueryChannelsEndpoint)),
                Arg.Is<object>(body => RequestBodyContains(body, "messaging:b")
                                       && !RequestBodyContains(body, "messaging:a")));
        }

        [Test]
        public void when_sync_returns_empty_events_with_inaccessible_cids_expect_those_cids_not_requeried()
        {
            Connect();
            WatchChannel("messaging:gone");
            var ok = WatchChannel("messaging:ok");

            var now = new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero);
            _mockTimeService.Now.Returns(now);

            DropConnection();
            SetDisconnectionLastEventReceivedAt(_client.InternalLowLevelClient, now.AddHours(-1));

            RespondWith(SyncEndpoint, "{\"events\":[],\"inaccessible_cids\":[\"messaging:gone\"]}");
            RespondWith(QueryChannelsEndpoint, QueryChannelsJson("messaging:ok", "ok"));

            Reconnect();

            _mockHttpClient.Received(1).SendHttpRequestAsync(
                Arg.Is(HttpMethodType.Post),
                Arg.Is<Uri>(uri => uri.AbsolutePath.EndsWith(QueryChannelsEndpoint)),
                Arg.Is<object>(body => RequestBodyContains(body, "messaging:ok")
                                       && !RequestBodyContains(body, "messaging:gone")));

            Assert.AreEqual(1, _recoveredEvents.Count);
            Assert.AreSame(ok, _recoveredEvents[0].Channels.Single());
            Assert.AreEqual(new[] { "messaging:gone" }, _recoveredEvents[0].UnrecoveredChannelCids.ToArray());
            Assert.IsFalse(_recoveredEvents[0].IsComplete);
            Assert.IsTrue(ok.IsWatched);
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

        [Test]
        public void when_silent_history_batch_trims_expect_messages_removed_from_cache_not_raised()
        {
            Connect();
            var channel = WatchChannel("messaging:a");
            channel.OverrideMessageCacheWindow(SmallWindow);

            var removedCount = 0;
            channel.MessagesRemovedFromCache += (_, __) => removedCount++;

            _client.InternalLowLevelClient.ApplyHistoryEvents(MessageNewEvents("messaging:a", count: 7));

            Assert.AreEqual(0, removedCount);
            Assert.AreEqual(SmallWindow.MaxMessages - SmallWindow.DiscardBatchSize, channel.Messages.Count);
        }

        [Test]
        public void when_history_replay_trims_expect_messages_removed_from_cache_raised()
        {
            Connect();
            var channel = WatchChannel("messaging:a");
            channel.OverrideMessageCacheWindow(SmallWindow);

            var removedCount = 0;
            channel.MessagesRemovedFromCache += (_, __) => removedCount++;

            _client.InternalLowLevelClient.ReplayHistoryEvents(MessageNewEvents("messaging:a", count: 7));

            Assert.AreEqual(1, removedCount);
            Assert.AreEqual(SmallWindow.MaxMessages - SmallWindow.DiscardBatchSize, channel.Messages.Count);
        }

        [Test]
        public void when_silent_history_batch_adds_thread_reply_expect_reply_received_not_raised()
        {
            Connect();
            WatchChannel("messaging:a");
            var thread = TrackThread("messaging:a", "parent-1");

            var replyCount = 0;
            thread.ReplyReceived += (_, __) => replyCount++;

            _client.InternalLowLevelClient.ApplyHistoryEvents(new[]
            {
                ThreadReplyJson("messaging:a", "parent-1", "reply-1"),
            });

            Assert.AreEqual(0, replyCount);
            Assert.AreEqual(1, thread.LatestReplies.Count);
            Assert.AreEqual("reply-1", thread.LatestReplies[0].Id);
        }

        [Test]
        public void when_history_replay_adds_thread_reply_expect_reply_received_raised()
        {
            Connect();
            WatchChannel("messaging:a");
            var thread = TrackThread("messaging:a", "parent-1");

            var replyCount = 0;
            thread.ReplyReceived += (_, __) => replyCount++;

            _client.InternalLowLevelClient.ReplayHistoryEvents(new[]
            {
                ThreadReplyJson("messaging:a", "parent-1", "reply-1"),
            });

            Assert.AreEqual(1, replyCount);
            Assert.AreEqual(1, thread.LatestReplies.Count);
            Assert.AreEqual("reply-1", thread.LatestReplies[0].Id);
        }

        [Test]
        public void when_silent_history_batch_changes_presence_expect_presence_changed_not_raised()
        {
            Connect();
            var user = TrackUser("other-user", online: false);

            var presenceCount = 0;
            user.PresenceChanged += (_, __, ___) => presenceCount++;

            _client.InternalLowLevelClient.ApplyHistoryEvents(new[]
            {
                PresenceChangedJson("other-user", online: true),
            });

            Assert.AreEqual(0, presenceCount);
            Assert.IsTrue(user.Online);
        }

        [Test]
        public void when_history_replay_changes_presence_expect_presence_changed_raised()
        {
            Connect();
            var user = TrackUser("other-user", online: false);

            var presenceCount = 0;
            user.PresenceChanged += (_, __, ___) => presenceCount++;

            _client.InternalLowLevelClient.ReplayHistoryEvents(new[]
            {
                PresenceChangedJson("other-user", online: true),
            });

            Assert.AreEqual(1, presenceCount);
            Assert.IsTrue(user.Online);
        }

        [Test]
        public void when_silent_history_batch_closes_poll_expect_closed_not_raised()
        {
            Connect();
            WatchChannel("messaging:a");
            var poll = TrackPoll("poll-1");

            var closedCount = 0;
            var updatedCount = 0;
            poll.Closed += _ => closedCount++;
            poll.Updated += _ => updatedCount++;

            _client.InternalLowLevelClient.ApplyHistoryEvents(new[]
            {
                PollClosedJson("messaging:a", "poll-1"),
            });

            Assert.AreEqual(0, closedCount);
            Assert.AreEqual(0, updatedCount);
            Assert.IsTrue(poll.IsClosed);
        }

        [Test]
        public void when_history_replay_closes_poll_expect_closed_raised()
        {
            Connect();
            WatchChannel("messaging:a");
            var poll = TrackPoll("poll-1");

            var closedCount = 0;
            poll.Closed += _ => closedCount++;

            _client.InternalLowLevelClient.ReplayHistoryEvents(new[]
            {
                PollClosedJson("messaging:a", "poll-1"),
            });

            Assert.AreEqual(1, closedCount);
            Assert.IsTrue(poll.IsClosed);
        }

        [Test]
        public void when_silent_recovery_syncs_message_new_expect_channel_message_received_not_raised()
        {
            _config.StateRecoveryStrategy = StateRecoveryStrategy.BatchStateUpdate;

            Connect();
            var channel = WatchChannel("messaging:a");
            RespondWith(QueryChannelsEndpoint, QueryChannelsJson("messaging:a", "a"));

            var receivedCount = 0;
            channel.MessageReceived += (_, __) => receivedCount++;

            ReconnectWithSync(SyncEventsJson(MessageNewJson("messaging:a", "msg-offline",
                new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero))));

            Assert.AreEqual(0, receivedCount);
            Assert.AreEqual(1, channel.Messages.Count);
            Assert.AreEqual("msg-offline", channel.Messages[0].Id);
            Assert.AreEqual(1, _recoveredEvents.Count);
        }

        [Test]
        public void when_replay_recovery_syncs_message_new_expect_channel_message_received_raised()
        {
            Connect();
            var channel = WatchChannel("messaging:a");
            RespondWith(QueryChannelsEndpoint, QueryChannelsJson("messaging:a", "a"));

            var receivedCount = 0;
            channel.MessageReceived += (_, __) => receivedCount++;

            ReconnectWithSync(SyncEventsJson(MessageNewJson("messaging:a", "msg-offline",
                new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero))));

            Assert.AreEqual(1, receivedCount);
            Assert.AreEqual(1, channel.Messages.Count);
            Assert.AreEqual("msg-offline", channel.Messages[0].Id);
            Assert.AreEqual(1, _recoveredEvents.Count);
        }

        [Test]
        public void when_replay_recovery_syncs_many_events_expect_they_span_updates()
        {
            _client.InternalLowLevelClient.EventDrainCountCap = 2;
            _client.InternalLowLevelClient.EventDrainTimeBudgetMs = 10_000;

            Connect();
            var channel = WatchChannel("messaging:a");
            RespondWith(QueryChannelsEndpoint, QueryChannelsJson("messaging:a", "a"));

            var received = 0;
            channel.MessageReceived += (_, __) => received++;

            ReconnectWithSync(ManySyncMessages("messaging:a", count: 5));

            Assert.AreEqual(1, received, "Health check consumes one slot of the cap; only one history event fits.");
            AssertQueryChannelsCallCount(0);
            Assert.AreEqual(0, _recoveredEvents.Count);

            Update();
            Assert.AreEqual(3, received);
            AssertQueryChannelsCallCount(0);
            Assert.AreEqual(0, _recoveredEvents.Count);

            Update();
            Assert.AreEqual(5, received);
            AssertQueryChannelsCallCount(1);
            Assert.AreEqual(1, _recoveredEvents.Count);
        }

        [Test]
        public void when_silent_recovery_syncs_many_events_expect_they_span_updates_without_callbacks()
        {
            _config.StateRecoveryStrategy = StateRecoveryStrategy.BatchStateUpdate;
            _client.InternalLowLevelClient.EventDrainCountCap = 2;
            _client.InternalLowLevelClient.EventDrainTimeBudgetMs = 10_000;

            Connect();
            var channel = WatchChannel("messaging:a");
            RespondWith(QueryChannelsEndpoint, QueryChannelsJson("messaging:a", "a"));

            var received = 0;
            channel.MessageReceived += (_, __) => received++;

            ReconnectWithSync(ManySyncMessages("messaging:a", count: 5));

            Assert.AreEqual(0, received);
            Assert.AreEqual(1, channel.Messages.Count);
            AssertQueryChannelsCallCount(0);
            Assert.AreEqual(0, _recoveredEvents.Count);
            Assert.IsNull(GetLastEventReceivedAt(_client.InternalLowLevelClient));

            Update();
            Assert.AreEqual(0, received);
            Assert.AreEqual(3, channel.Messages.Count);
            AssertQueryChannelsCallCount(0);
            Assert.IsNull(GetLastEventReceivedAt(_client.InternalLowLevelClient));

            Update();
            Assert.AreEqual(0, received);
            Assert.AreEqual(5, channel.Messages.Count);
            AssertQueryChannelsCallCount(1);
            Assert.AreEqual(1, _recoveredEvents.Count);
            Assert.AreEqual(new DateTimeOffset(2026, 8, 24, 12, 0, 4, TimeSpan.Zero),
                GetLastEventReceivedAt(_client.InternalLowLevelClient));
        }

        [Test]
        public void when_disconnect_mid_paced_recovery_expect_no_state_recovered()
        {
            _client.InternalLowLevelClient.EventDrainCountCap = 1;
            _client.InternalLowLevelClient.EventDrainTimeBudgetMs = 10_000;

            Connect();
            WatchChannel("messaging:a");
            RespondWith(QueryChannelsEndpoint, QueryChannelsJson("messaging:a", "a"));

            ReconnectWithSync(ManySyncMessages("messaging:a", count: 5));
            Assert.AreEqual(0, _recoveredEvents.Count);
            AssertQueryChannelsCallCount(0);

            DropConnection();
            Update();
            Update();

            Assert.AreEqual(0, _recoveredEvents.Count);
            AssertQueryChannelsCallCount(0);
        }

        [Test]
        public void when_silent_recovery_syncs_custom_event_expect_channel_custom_event_received()
        {
            _config.StateRecoveryStrategy = StateRecoveryStrategy.BatchStateUpdate;

            Connect();
            var channel = WatchChannel("messaging:a");
            RespondWith(QueryChannelsEndpoint, QueryChannelsJson("messaging:a", "a"));

            string receivedType = null;
            channel.CustomEventReceived += (_, evt) => receivedType = evt.Type;

            ReconnectWithSync(SyncEventsJson(CustomEventJson("messaging:a", "game.state")));

            Assert.AreEqual("game.state", receivedType);
        }

        private const string SyncEndpoint = "/sync";
        private const string QueryChannelsEndpoint = "/channels";

        // Same window as MessageCacheWindowTests: 7 messages exceed MaxMessages and trim down to 3.
        private static readonly MessageCacheWindow SmallWindow = new MessageCacheWindow(6, 3);

        private static IEnumerable<object> MessageNewEvents(string cid, int count)
        {
            var start = new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero);
            var events = new List<object>(count);
            for (var i = 0; i < count; i++)
            {
                events.Add(MessageNewJson(cid, $"msg-{i}", start.AddSeconds(i)));
            }

            return events;
        }

        private static string MessageNewJson(string cid, string messageId, DateTimeOffset createdAt)
            => $"{{\"type\":\"message.new\",\"cid\":\"{cid}\",\"created_at\":\"{createdAt:O}\"," +
               $"\"message\":{{\"id\":\"{messageId}\",\"text\":\"hi\",\"created_at\":\"{createdAt:O}\"," +
               $"\"updated_at\":\"{createdAt:O}\",\"user\":{{\"id\":\"user-1\"}}}}}}";

        private static string ThreadReplyJson(string cid, string parentId, string replyId)
        {
            var createdAt = new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero);
            return $"{{\"type\":\"message.new\",\"cid\":\"{cid}\",\"created_at\":\"{createdAt:O}\"," +
                   $"\"message\":{{\"id\":\"{replyId}\",\"parent_id\":\"{parentId}\",\"text\":\"reply\"," +
                   $"\"created_at\":\"{createdAt:O}\",\"updated_at\":\"{createdAt:O}\"," +
                   $"\"user\":{{\"id\":\"user-1\"}}}}}}";
        }

        private static string PresenceChangedJson(string userId, bool online)
        {
            var createdAt = new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero);
            var onlineJson = online ? "true" : "false";
            return $"{{\"type\":\"user.presence.changed\",\"created_at\":\"{createdAt:O}\"," +
                   $"\"user\":{{\"id\":\"{userId}\",\"online\":{onlineJson}}}}}";
        }

        private static string PollClosedJson(string cid, string pollId)
        {
            var createdAt = new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero);
            return $"{{\"type\":\"poll.closed\",\"cid\":\"{cid}\",\"created_at\":\"{createdAt:O}\"," +
                   $"\"poll\":{{\"id\":\"{pollId}\",\"name\":\"q\",\"is_closed\":true,\"vote_count\":0," +
                   $"\"voting_visibility\":\"public\"}}}}";
        }

        private static string CustomEventJson(string cid, string type)
        {
            var createdAt = new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero);
            return $"{{\"type\":\"{type}\",\"cid\":\"{cid}\",\"created_at\":\"{createdAt:O}\"," +
                   "\"user\":{\"id\":\"user-1\"}}";
        }

        private static string SyncEventsJson(params string[] events)
            => "{\"events\":[" + string.Join(",", events) + "]}";

        private static string ManySyncMessages(string cid, int count)
        {
            var start = new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero);
            var events = new string[count];
            for (var i = 0; i < count; i++)
            {
                events[i] = MessageNewJson(cid, $"msg-{i}", start.AddSeconds(i));
            }

            return SyncEventsJson(events);
        }

        private static DateTimeOffset? GetLastEventReceivedAt(StreamChatLowLevelClient client)
        {
            var field = typeof(StreamChatLowLevelClient).GetField("_lastEventReceivedAt",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, "Expected _lastEventReceivedAt to exist.");
            return (DateTimeOffset?)field.GetValue(client);
        }

        private void ReconnectWithSync(string syncJson)
        {
            var now = new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero);
            _mockTimeService.Now.Returns(now);

            DropConnection();
            SetDisconnectionLastEventReceivedAt(_client.InternalLowLevelClient, now.AddHours(-1));
            RespondWith(SyncEndpoint, syncJson);
            Reconnect();
        }

        private IStreamThread TrackThread(string cid, string parentMessageId)
            => _client.InternalCache.TryCreateOrUpdate(new ThreadStateInternalDTO
            {
                ParentMessageId = parentMessageId,
                ChannelCid = cid,
                ReplyCount = 0,
                ParentMessage = new MessageInternalDTO
                {
                    Id = parentMessageId,
                    Text = "parent",
                    User = new UserObjectInternalDTO { Id = "user-1" },
                },
            });

        private IStreamUser TrackUser(string userId, bool online)
            => _client.InternalCache.TryCreateOrUpdate(new UserObjectInternalDTO
            {
                Id = userId,
                Online = online,
            });

        private IStreamPoll TrackPoll(string pollId)
            => _client.InternalCache.TryCreateOrUpdate(new PollResponseDataInternalDTO
            {
                Id = pollId,
                Name = "q",
                IsClosed = false,
                VoteCount = 0,
            });

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

        private static void SetDisconnectionLastEventReceivedAt(StreamChatLowLevelClient client, DateTimeOffset value)
        {
            var field = typeof(StreamChatLowLevelClient).GetField("_disconnectionLastEventReceivedAt",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, "Expected _disconnectionLastEventReceivedAt to exist.");
            field.SetValue(client, (DateTimeOffset?)value);
        }

        private static bool RequestBodyContains(object requestBody, string value)
        {
            var json = requestBody as string ?? requestBody?.ToString() ?? string.Empty;
            return json.IndexOf(value, StringComparison.Ordinal) >= 0;
        }

        private static bool RequestHasJsonBool(object requestBody, string property, bool value)
        {
            var json = requestBody as string ?? requestBody?.ToString() ?? string.Empty;
            var needle = "\"" + property + "\":" + (value ? "true" : "false");
            return json.IndexOf(needle, StringComparison.Ordinal) >= 0;
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
