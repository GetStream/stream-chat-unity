#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Channels;
using StreamChat.Core.QueryBuilders.Filters.Messages;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Serialization;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests for <see cref="IStreamChatClient.SearchMessagesAsync"/>.
    ///
    /// <para>
    /// Coverage matches the test plan in <c>docs/specs/search-messages.md</c>:
    /// integration scenarios for the most important use cases plus client-side validation
    /// rules and filter / sort builder shape assertions.
    /// </para>
    /// </summary>
    internal class SearchMessagesTests : BaseStateIntegrationTests
    {
        // ---------------------------------------------------------------------
        // Builder shape tests (no live server, no connection required)
        // ---------------------------------------------------------------------

        [Test]
        public void When_message_filter_mentioned_user_id_contains_then_field_and_operator_are_correct()
        {
            var entry = MessageFilter.MentionedUserId.Contains("bob").GenerateFilterEntry();
            Assert.AreEqual("mentioned_users.id", entry.Key);
            AssertOperator(entry, "$contains", "bob");
        }

        [Test]
        public void When_message_filter_attachment_type_in_then_field_and_operator_are_correct()
        {
            var rule = MessageFilter.AttachmentType.In(new[] { "image", "video" });
            Assert.AreEqual("attachments.type", rule.Field);

            var entry = rule.GenerateFilterEntry();
            AssertOperator(entry, "$in", new[] { "image", "video" });
        }

        [Test]
        public void When_message_filter_created_at_gte_then_field_and_operator_are_correct()
        {
            var when = new DateTimeOffset(2024, 1, 15, 10, 0, 0, TimeSpan.Zero);
            var rule = MessageFilter.CreatedAt.GreaterThanOrEquals(when);
            Assert.AreEqual("created_at", rule.Field);

            var entry = rule.GenerateFilterEntry();
            var op = (IDictionary<string, object>)entry.Value;
            Assert.IsTrue(op.ContainsKey("$gte"));
        }

        [Test]
        public void When_message_filter_parent_id_exists_then_field_and_operator_are_correct()
        {
            var entry = MessageFilter.ParentId.Exists(true).GenerateFilterEntry();
            Assert.AreEqual("parent_id", entry.Key);
            AssertOperator(entry, "$exists", true);
        }

        [Test]
        public void When_message_filter_pinned_equals_then_field_and_operator_are_correct()
        {
            var entry = MessageFilter.Pinned.EqualsTo(true).GenerateFilterEntry();
            Assert.AreEqual("pinned", entry.Key);
            AssertOperator(entry, "$eq", true);
        }

        [Test]
        public void When_message_filter_silent_equals_then_field_and_operator_are_correct()
        {
            var entry = MessageFilter.Silent.EqualsTo(false).GenerateFilterEntry();
            Assert.AreEqual("silent", entry.Key);
            AssertOperator(entry, "$eq", false);
        }

        [Test]
        public void When_message_filter_type_equals_then_field_and_operator_are_correct()
        {
            var entry = MessageFilter.Type.EqualsTo("regular").GenerateFilterEntry();
            Assert.AreEqual("type", entry.Key);
            AssertOperator(entry, "$eq", "regular");
        }

        [Test]
        public void When_message_filter_user_id_in_then_field_and_operator_are_correct()
        {
            var entry = MessageFilter.UserId.In(new[] { "alice", "bob" }).GenerateFilterEntry();
            Assert.AreEqual("user.id", entry.Key);
            AssertOperator(entry, "$in", new[] { "alice", "bob" });
        }

        [Test]
        public void When_message_filter_custom_field_equals_then_uses_supplied_field_name()
        {
            var entry = MessageFilter.Custom("priority").EqualsTo("high").GenerateFilterEntry();
            Assert.AreEqual("priority", entry.Key);
            AssertOperator(entry, "$eq", "high");
        }

        [Test]
        public void When_messages_sort_order_by_descending_created_at_then_dto_contains_field_and_minus_one_direction()
        {
            var sort = MessagesSort.OrderByDescending(MessageSortFieldName.CreatedAt);
            var dto = sort.ToSortParamRequestList();

            Assert.IsNotNull(dto);
            Assert.AreEqual(1, dto.Count);
            Assert.AreEqual("created_at", dto[0].Field);
            Assert.AreEqual(-1, dto[0].Direction);
        }

        [Test]
        public void When_messages_sort_then_by_ascending_then_dto_contains_both_entries_in_order()
        {
            var sort = MessagesSort
                .OrderByDescending(MessageSortFieldName.CreatedAt)
                .ThenByAscending(MessageSortFieldName.Id);

            var dto = sort.ToSortParamRequestList();

            Assert.IsNotNull(dto);
            Assert.AreEqual(2, dto.Count);
            Assert.AreEqual("created_at", dto[0].Field);
            Assert.AreEqual(-1, dto[0].Direction);
            Assert.AreEqual("id", dto[1].Field);
            Assert.AreEqual(1, dto[1].Direction);
        }

        [Test]
        public void When_request_save_to_dto_then_channel_and_message_filters_are_separated()
        {
            var request = new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo("messaging:abc"),
                },
                MessageFilter = new IFieldFilterRule[]
                {
                    MessageFilter.MentionedUserId.Contains("bob"),
                },
                Query = "hello",
                Limit = 30,
                Offset = 0,
            };

            var dto = ((ISavableTo<global::StreamChat.Core.InternalDTO.Requests.SearchRequestInternalDTO>)request)
                .SaveToDto();

            Assert.IsNotNull(dto.FilterConditions);
            Assert.IsTrue(dto.FilterConditions.ContainsKey("cid"));

            Assert.IsNotNull(dto.MessageFilterConditions);
            Assert.IsTrue(dto.MessageFilterConditions.ContainsKey("mentioned_users.id"));

            Assert.AreEqual("hello", dto.Query);
            Assert.AreEqual(30, dto.Limit);
            Assert.AreEqual(0, dto.Offset);
        }

        [TestCase(0, TestName = "empty array")]
        [TestCase(1, TestName = "array of null rules")]
        public void When_request_with_query_and_no_effective_message_filter_then_wire_payload_omits_message_filter_conditions(
            int nullRuleCount)
        {
            var request = new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo("messaging:abc"),
                },
                MessageFilter = new IFieldFilterRule[nullRuleCount],
                Query = "hello",
            };

            var dto = ((ISavableTo<SearchRequestInternalDTO>)request).SaveToDto();
            Assert.IsNull(dto.MessageFilterConditions);

            var json = new NewtonsoftJsonSerializer().Serialize(dto);
            Assert.IsFalse(json.Contains("message_filter_conditions"),
                "Got payload: " + json);
        }

        // ---------------------------------------------------------------------
        // Client-side validation tests
        // ---------------------------------------------------------------------

        [UnityTest]
        public IEnumerator When_search_with_null_request_expect_throws()
            => ConnectAndExecute(When_search_with_null_request_expect_throws_Async);

        private async Task When_search_with_null_request_expect_throws_Async()
        {
            await AssertThrowsAsync<ArgumentNullException>(
                () => Client.SearchMessagesAsync(null));
        }

        [UnityTest]
        public IEnumerator When_search_with_null_channel_filter_expect_throws()
            => ConnectAndExecute(When_search_with_null_channel_filter_expect_throws_Async);

        private async Task When_search_with_null_channel_filter_expect_throws_Async()
        {
            await AssertThrowsAsync<ArgumentException>(
                () => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
                {
                    ChannelFilter = null,
                    Query = "anything",
                }));
        }

        [UnityTest]
        public IEnumerator When_search_with_empty_channel_filter_expect_throws()
            => ConnectAndExecute(When_search_with_empty_channel_filter_expect_throws_Async);

        private async Task When_search_with_empty_channel_filter_expect_throws_Async()
        {
            await AssertThrowsAsync<ArgumentException>(
                () => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
                {
                    ChannelFilter = new IFieldFilterRule[0],
                    Query = "anything",
                }));
        }

        [UnityTest]
        public IEnumerator When_search_with_offset_and_next_both_set_expect_throws()
            => ConnectAndExecute(When_search_with_offset_and_next_both_set_expect_throws_Async);

        private async Task When_search_with_offset_and_next_both_set_expect_throws_Async()
        {
            await AssertThrowsAsync<ArgumentException>(
                () => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
                {
                    ChannelFilter = new IFieldFilterRule[]
                    {
                        ChannelFilter.Members.In(Client.LocalUserData.User),
                    },
                    Query = "x",
                    Offset = 30,
                    Next = "fake-cursor",
                }));
        }

        [UnityTest]
        public IEnumerator When_search_with_sort_and_non_zero_offset_expect_throws()
            => ConnectAndExecute(When_search_with_sort_and_non_zero_offset_expect_throws_Async);

        private async Task When_search_with_sort_and_non_zero_offset_expect_throws_Async()
        {
            await AssertThrowsAsync<ArgumentException>(
                () => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
                {
                    ChannelFilter = new IFieldFilterRule[]
                    {
                        ChannelFilter.Members.In(Client.LocalUserData.User),
                    },
                    Query = "x",
                    Offset = 30,
                    Sort = MessagesSort.OrderByDescending(MessageSortFieldName.CreatedAt),
                }));
        }

        [UnityTest]
        public IEnumerator When_search_with_query_and_text_message_filter_expect_throws()
            => ConnectAndExecute(When_search_with_query_and_text_message_filter_expect_throws_Async);

        private async Task When_search_with_query_and_text_message_filter_expect_throws_Async()
        {
            await AssertThrowsAsync<ArgumentException>(
                () => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
                {
                    ChannelFilter = new IFieldFilterRule[]
                    {
                        ChannelFilter.Members.In(Client.LocalUserData.User),
                    },
                    Query = "hello",
                    MessageFilter = new IFieldFilterRule[]
                    {
                        MessageFilter.Text.Contains("hello"),
                    },
                }));
        }

        [UnityTest]
        public IEnumerator When_search_with_query_and_non_text_message_filter_expect_throws()
            => ConnectAndExecute(When_search_with_query_and_non_text_message_filter_expect_throws_Async);

        private async Task When_search_with_query_and_non_text_message_filter_expect_throws_Async()
        {
            // Server rejects ANY combination of `query` + `message_filter_conditions`, not just on
            // the `text` field. The client must surface that as ArgumentException up-front so callers
            // don't get a confusing 400 from the server.
            await AssertThrowsAsync<ArgumentException>(
                () => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
                {
                    ChannelFilter = new IFieldFilterRule[]
                    {
                        ChannelFilter.Members.In(Client.LocalUserData.User),
                    },
                    Query = "hello",
                    MessageFilter = new IFieldFilterRule[]
                    {
                        MessageFilter.ParentId.Exists(false),
                    },
                }));
        }

        [UnityTest]
        public IEnumerator When_search_with_limit_below_one_expect_throws()
            => ConnectAndExecute(When_search_with_limit_below_one_expect_throws_Async);

        private async Task When_search_with_limit_below_one_expect_throws_Async()
        {
            await AssertThrowsAsync<ArgumentOutOfRangeException>(
                () => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
                {
                    ChannelFilter = new IFieldFilterRule[]
                    {
                        ChannelFilter.Members.In(Client.LocalUserData.User),
                    },
                    Query = "x",
                    Limit = 0,
                }));
        }

        [UnityTest]
        public IEnumerator When_search_with_cancelled_token_expect_throws_operation_cancelled()
            => ConnectAndExecute(When_search_with_cancelled_token_expect_throws_operation_cancelled_Async);

        private async Task When_search_with_cancelled_token_expect_throws_operation_cancelled_Async()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            await AssertThrowsAsync<OperationCanceledException>(
                () => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
                {
                    ChannelFilter = new IFieldFilterRule[]
                    {
                        ChannelFilter.Members.In(Client.LocalUserData.User),
                    },
                    Query = "x",
                }, cts.Token));
        }

        // ---------------------------------------------------------------------
        // Integration tests (require live server)
        // ---------------------------------------------------------------------

        [UnityTest]
        public IEnumerator When_search_by_mentioned_user_id_expect_only_messages_mentioning_that_user()
            => ConnectAndExecute(When_search_by_mentioned_user_id_expect_only_messages_mentioning_that_user_Async);

        private async Task When_search_by_mentioned_user_id_expect_only_messages_mentioning_that_user_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var userToMention = await CreateUniqueTempUserAsync("Michael");

            await channel.SendNewMessageAsync("Hello");
            await channel.SendNewMessageAsync("How are you");
            var mentionMessage = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = "Hey there!",
                MentionedUsers = new List<IStreamUser> { userToMention },
            });

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                MessageFilter = new IFieldFilterRule[]
                {
                    MessageFilter.MentionedUserId.Contains(userToMention.Id),
                },
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == mentionMessage.Id));

            Assert.IsNotEmpty(response.Results);
            var hit = response.Results.FirstOrDefault(r => r.Message.Id == mentionMessage.Id);
            Assert.IsNotNull(hit, "Expected to find the mention message in search results.");

            // Spec 4.3 + 5.3: results expose stateful IStreamMessage + IStreamChannel.
            Assert.IsInstanceOf<IStreamMessage>(hit.Message);
            Assert.IsInstanceOf<IStreamChannel>(hit.Channel);
            Assert.AreEqual(channel.Cid, hit.Channel.Cid);
        }

        [UnityTest]
        public IEnumerator When_search_with_query_and_empty_message_filter_expect_success()
            => ConnectAndExecute(When_search_with_query_and_empty_message_filter_expect_success_Async);

        private async Task When_search_with_query_and_empty_message_filter_expect_success_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var token = "emptyfilter-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var sent = await channel.SendNewMessageAsync(token);

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                MessageFilter = new IFieldFilterRule[0],
                Query = token,
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == sent.Id));

            Assert.IsTrue(response.Results.Any(r => r.Message.Id == sent.Id));
        }

        [UnityTest]
        public IEnumerator When_search_with_query_text_expect_matching_message_returned()
            => ConnectAndExecute(When_search_with_query_text_expect_matching_message_returned_Async);

        private async Task When_search_with_query_text_expect_matching_message_returned_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var unique = "needle-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var matching = await channel.SendNewMessageAsync("Special content with " + unique);
            await channel.SendNewMessageAsync("Plain message without the token");

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                Query = unique,
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == matching.Id));

            Assert.IsTrue(response.Results.Any(r => r.Message.Id == matching.Id));
            Assert.IsTrue(response.Results.All(r => r.Channel != null && r.Channel.Cid == channel.Cid),
                "All hits should belong to the requested channel.");
        }

        [UnityTest]
        public IEnumerator When_search_restricted_by_single_cid_expect_only_that_channels_messages()
            => ConnectAndExecute(When_search_restricted_by_single_cid_expect_only_that_channels_messages_Async);

        private async Task When_search_restricted_by_single_cid_expect_only_that_channels_messages_Async()
        {
            var channelA = await CreateUniqueTempChannelAsync();
            var channelB = await CreateUniqueTempChannelAsync();

            var token = "scoped-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var msgInA = await channelA.SendNewMessageAsync("In A: " + token);
            await channelB.SendNewMessageAsync("In B: " + token);

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channelA.Cid),
                },
                Query = token,
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == msgInA.Id));

            Assert.IsNotEmpty(response.Results);
            Assert.IsTrue(response.Results.All(r => r.Channel.Cid == channelA.Cid),
                "Only messages from channelA should be returned when Cid filter restricts to it.");
        }

        [UnityTest]
        public IEnumerator When_search_returns_message_already_in_watched_channel_expect_same_instance()
            => ConnectAndExecute(When_search_returns_message_already_in_watched_channel_expect_same_instance_Async);

        private async Task When_search_returns_message_already_in_watched_channel_expect_same_instance_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var token = "identity-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var sent = await channel.SendNewMessageAsync(token);

            // The sent message lives in channel.Messages (the channel is watched).
            var cached = channel.Messages.First(m => m.Id == sent.Id);
            Assert.AreSame(sent, cached);

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                Query = token,
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == sent.Id));

            var hit = response.Results.First(r => r.Message.Id == sent.Id);

            // Spec 6.1: cache identity - the search hit is the exact same instance.
            Assert.AreSame(cached, hit.Message,
                "Search hit Message should be the same cached instance as channel.Messages.");
            Assert.AreSame(channel, hit.Channel,
                "Search hit Channel should be the same cached instance as the watched channel.");
        }

        [UnityTest]
        public IEnumerator When_search_with_custom_field_filter_expect_matching_messages()
            => ConnectAndExecute(When_search_with_custom_field_filter_expect_matching_messages_Async);

        private async Task When_search_with_custom_field_filter_expect_matching_messages_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            // Use a unique custom field name per run to avoid cross-test interference on shared indices.
            var customKey = "test_priority_" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var high = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = "High priority message",
                CustomData = new StreamCustomDataRequest { { customKey, "high" } }
            });

            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = "Low priority message",
                CustomData = new StreamCustomDataRequest { { customKey, "low" } }
            });

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                MessageFilter = new IFieldFilterRule[]
                {
                    MessageFilter.Custom(customKey).EqualsTo("high"),
                },
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == high.Id));

            Assert.IsTrue(response.Results.Any(r => r.Message.Id == high.Id));
            Assert.IsTrue(response.Results.All(r =>
                    r.Message.CustomData != null &&
                    r.Message.CustomData.Get<string>(customKey) == "high"),
                "Every result should have the custom field set to 'high'.");
        }

        [UnityTest]
        public IEnumerator When_search_with_parent_id_exists_true_expect_only_replies()
            => ConnectAndExecute(When_search_with_parent_id_exists_true_expect_only_replies_Async);

        private async Task When_search_with_parent_id_exists_true_expect_only_replies_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var token = "replies-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var parent = await channel.SendNewMessageAsync("Parent " + token);
            var reply = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = "Reply " + token,
                ParentId = parent.Id,
            });

            // Note: cannot combine Query with MessageFilter (server rejects it). The unique
            // channel scope is sufficient to isolate this test's messages.
            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                MessageFilter = new IFieldFilterRule[]
                {
                    MessageFilter.ParentId.Exists(true),
                },
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == reply.Id));

            Assert.IsTrue(response.Results.Any(r => r.Message.Id == reply.Id));
            Assert.IsTrue(response.Results.All(r => !string.IsNullOrEmpty(r.Message.ParentId)),
                "ParentId.Exists(true) should only return reply messages.");
            Assert.IsFalse(response.Results.Any(r => r.Message.Id == parent.Id),
                "Parent message should not be returned when filtering for replies only.");
        }

        [UnityTest]
        public IEnumerator When_search_with_parent_id_exists_false_expect_only_top_level_messages()
            => ConnectAndExecute(When_search_with_parent_id_exists_false_expect_only_top_level_messages_Async);

        private async Task When_search_with_parent_id_exists_false_expect_only_top_level_messages_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var token = "toplevel-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var parent = await channel.SendNewMessageAsync("Parent " + token);
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = "Reply " + token,
                ParentId = parent.Id,
            });

            // Note: cannot combine Query with MessageFilter (server rejects it). The unique
            // channel scope is sufficient to isolate this test's messages.
            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                MessageFilter = new IFieldFilterRule[]
                {
                    MessageFilter.ParentId.Exists(false),
                },
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == parent.Id));

            Assert.IsTrue(response.Results.Any(r => r.Message.Id == parent.Id));
            Assert.IsTrue(response.Results.All(r => string.IsNullOrEmpty(r.Message.ParentId)),
                "ParentId.Exists(false) should only return top-level (non-reply) messages.");
        }

        [UnityTest]
        public IEnumerator When_search_with_date_range_expect_only_messages_within_range()
            => ConnectAndExecute(When_search_with_date_range_expect_only_messages_within_range_Async);

        private async Task When_search_with_date_range_expect_only_messages_within_range_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var token = "daterange-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var lowerBound = DateTimeOffset.UtcNow.AddMinutes(-2);
            var msg = await channel.SendNewMessageAsync("In window: " + token);

            // Note: cannot combine Query with MessageFilter (server rejects it). The unique
            // channel scope is sufficient to isolate this test's messages.
            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                MessageFilter = new IFieldFilterRule[]
                {
                    MessageFilter.CreatedAt.GreaterThanOrEquals(lowerBound),
                },
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == msg.Id));

            Assert.IsTrue(response.Results.Any(r => r.Message.Id == msg.Id));
            Assert.IsTrue(response.Results.All(r => r.Message.CreatedAt >= lowerBound.AddSeconds(-5)),
                "All returned messages must have CreatedAt >= the lower bound (small allowance for clock skew).");
        }

        [UnityTest]
        public IEnumerator When_search_with_sort_descending_expect_results_monotonically_decreasing()
            => ConnectAndExecute(When_search_with_sort_descending_expect_results_monotonically_decreasing_Async);

        private async Task When_search_with_sort_descending_expect_results_monotonically_decreasing_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var token = "sortdesc-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var sentIds = new List<string>();
            for (var i = 0; i < 3; i++)
            {
                var m = await channel.SendNewMessageAsync("Msg " + i + " " + token);
                sentIds.Add(m.Id);
            }

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                Query = token,
                Sort = MessagesSort.OrderByDescending(MessageSortFieldName.CreatedAt),
            }), r => r != null && r.Results != null && r.Results.Count >= sentIds.Count);

            // We only assert ordering on results that belong to this run (matched by the unique token).
            var ours = response.Results
                .Where(r => sentIds.Contains(r.Message.Id))
                .ToList();

            Assert.AreEqual(sentIds.Count, ours.Count, "Expected all messages from this run to be returned.");

            for (var i = 1; i < ours.Count; i++)
            {
                Assert.IsTrue(ours[i - 1].Message.CreatedAt >= ours[i].Message.CreatedAt,
                    "Descending sort: each subsequent CreatedAt should be <= the previous.");
            }
        }

        [UnityTest]
        public IEnumerator When_search_with_limit_then_response_capped_at_limit()
            => ConnectAndExecute(When_search_with_limit_then_response_capped_at_limit_Async);

        private async Task When_search_with_limit_then_response_capped_at_limit_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var token = "limit-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            for (var i = 0; i < 3; i++)
            {
                await channel.SendNewMessageAsync("Msg " + i + " " + token);
            }

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                Query = token,
                Limit = 1,
            }), r => r != null && r.Results != null && r.Results.Count >= 1);

            Assert.AreEqual(1, response.Results.Count,
                "Limit=1 should return at most one message per page.");
        }

        [UnityTest]
        public IEnumerator When_search_with_cursor_pagination_expect_next_cursor_and_disjoint_pages()
            => ConnectAndExecute(When_search_with_cursor_pagination_expect_next_cursor_and_disjoint_pages_Async);

        private async Task When_search_with_cursor_pagination_expect_next_cursor_and_disjoint_pages_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var token = "cursor-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var sentIds = new List<string>();
            for (var i = 0; i < 3; i++)
            {
                var m = await channel.SendNewMessageAsync("Msg " + i + " " + token);
                sentIds.Add(m.Id);
            }

            // Stream's search index is eventually consistent. Wait until ALL three messages
            // are searchable BEFORE testing cursor pagination - otherwise the server happily
            // returns a single result without a `next` cursor (because, from its point of view,
            // there are no more pages yet) and the cursor predicate below would race against
            // indexing for a long time.
            await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                Query = token,
                Limit = 30,
            }), r => r != null && r.Results != null &&
                     sentIds.All(id => r.Results.Any(x => x.Message != null && x.Message.Id == id)),
                description: "all 3 messages to be indexed for cursor pagination test");

            var page1 = await Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                Query = token,
                Limit = 1,
                Sort = MessagesSort.OrderByAscending(MessageSortFieldName.CreatedAt),
            });

            Assert.IsNotNull(page1);
            Assert.AreEqual(1, page1.Results.Count, "Page 1 must contain exactly Limit=1 result.");
            Assert.IsFalse(string.IsNullOrEmpty(page1.Next), "Page 1 must return a Next cursor.");

            var page2 = await Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                Query = token,
                Limit = 1,
                Sort = MessagesSort.OrderByAscending(MessageSortFieldName.CreatedAt),
                Next = page1.Next,
            });

            Assert.IsNotNull(page2);
            Assert.IsNotEmpty(page2.Results);

            var page1Ids = new HashSet<string>(page1.Results.Select(r => r.Message.Id));
            Assert.IsFalse(page2.Results.Any(r => page1Ids.Contains(r.Message.Id)),
                "Page 2 must not contain any message from page 1.");
        }

        [UnityTest]
        public IEnumerator When_search_returns_message_and_then_soft_deleted_expect_hit_reflects_deletion()
            => ConnectAndExecute(When_search_returns_message_and_then_soft_deleted_expect_hit_reflects_deletion_Async);

        private async Task When_search_returns_message_and_then_soft_deleted_expect_hit_reflects_deletion_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var token = "softdel-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var sent = await channel.SendNewMessageAsync(token);

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                Query = token,
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == sent.Id));

            var hit = response.Results.First(r => r.Message.Id == sent.Id);

            // Search results share cache identity with messages obtained through any other
            // surface (here: the freshly-sent `sent`). SoftDeleteAsync applies the REST
            // response to the cache before returning, so the search hit reflects the
            // deletion on the same instance with no WS round-trip needed.
            await sent.SoftDeleteAsync();

            Assert.IsTrue(hit.Message.IsDeleted, "Hit message IsDeleted should be true after soft-delete.");
            Assert.IsTrue(hit.Message.DeletedAt.HasValue);
            Assert.AreSame(sent, hit.Message, "Search hit and sent message must be the same cached instance.");
        }

        [UnityTest]
        public IEnumerator When_search_with_watch_result_channels_true_expect_channel_in_watched_channels()
            => ConnectAndExecute(When_search_with_watch_result_channels_true_expect_channel_in_watched_channels_Async);

        private async Task When_search_with_watch_result_channels_true_expect_channel_in_watched_channels_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var token = "watchtrue-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var msg = await channel.SendNewMessageAsync(token);

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                Query = token,
                WatchResultChannels = true,
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == msg.Id));

            Assert.IsTrue(response.Results.Any(r => r.Message.Id == msg.Id));
            Assert.IsTrue(Client.WatchedChannels.Any(c => c.Cid == channel.Cid),
                "With WatchResultChannels=true the hit channel must appear in WatchedChannels.");
            Assert.IsTrue(response.Results.First(r => r.Message.Id == msg.Id).Channel.IsWatched,
                "With WatchResultChannels=true the result Channel.IsWatched must be true.");
        }

        /// <summary>
        /// Verifies the core fix: a search hit's channel that is NOT already watched
        /// must NOT pollute <see cref="IStreamChatClient.WatchedChannels"/> when
        /// <see cref="StreamSearchMessagesRequest.WatchResultChannels"/> = false.
        ///
        /// <para>
        /// We use a second client to create the channel so the searching client has no
        /// prior cache entry for it; otherwise the channel would already be watched on
        /// the searching client and the test would be trivially true.
        /// </para>
        /// </summary>
        [UnityTest]
        public IEnumerator When_search_with_watch_result_channels_false_expect_channel_not_in_watched_channels()
            => ConnectAndExecute(When_search_with_watch_result_channels_false_expect_channel_not_in_watched_channels_Async);

        private async Task When_search_with_watch_result_channels_false_expect_channel_not_in_watched_channels_Async()
        {
            var otherClient = await GetConnectedOtherClientAsync();

            var channel = await CreateUniqueTempChannelAsync(overrideClient: otherClient);
            var token = "watchfalse-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var msg = await channel.SendNewMessageAsync(token);

            // Searching client has never interacted with this channel, so it must not
            // already be watched.
            Assert.IsFalse(Client.WatchedChannels.Any(c => c.Cid == channel.Cid),
                "Test precondition: searching client must not be watching the channel.");

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                Query = token,
                WatchResultChannels = false,
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == msg.Id));

            var hit = response.Results.First(r => r.Message.Id == msg.Id);

            Assert.IsNotNull(hit.Channel, "Hit Channel should still be returned even when not watched.");
            Assert.IsFalse(hit.Channel.IsWatched,
                "WatchResultChannels=false: hit Channel.IsWatched must be false.");
            Assert.IsFalse(Client.WatchedChannels.Any(c => c.Cid == channel.Cid),
                "WatchResultChannels=false: hit channel must NOT appear in WatchedChannels.");
        }

        /// <summary>
        /// If the channel is already watched (e.g. previously surfaced via QueryChannelsAsync
        /// or GetOrCreateChannelWithIdAsync), running a search with WatchResultChannels=false
        /// must NOT downgrade it to unwatched. The channel keeps receiving WS events.
        /// </summary>
        [UnityTest]
        public IEnumerator When_search_with_watch_result_channels_false_for_already_watched_channel_expect_still_watched()
            => ConnectAndExecute(
                When_search_with_watch_result_channels_false_for_already_watched_channel_expect_still_watched_Async);

        private async Task When_search_with_watch_result_channels_false_for_already_watched_channel_expect_still_watched_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var token = "alreadywatched-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var msg = await channel.SendNewMessageAsync(token);

            Assert.IsTrue(channel.IsWatched, "Test precondition: channel created with watch=true should be watched.");

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                Query = token,
                WatchResultChannels = false,
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == msg.Id));

            var hit = response.Results.First(r => r.Message.Id == msg.Id);
            Assert.IsTrue(hit.Channel.IsWatched,
                "Search must not downgrade an already-watched channel.");
            Assert.IsTrue(Client.WatchedChannels.Any(c => c.Cid == channel.Cid));
        }

        /// <summary>
        /// After SearchMessagesAsync(WatchResultChannels=false), a follow-up
        /// GetOrCreateChannelWithIdAsync on the same CID must promote the cached instance
        /// to watched (cache identity preserved across the transition).
        /// </summary>
        [UnityTest]
        public IEnumerator When_search_unwatched_channel_then_get_or_create_expect_same_instance_now_watched()
            => ConnectAndExecute(
                When_search_unwatched_channel_then_get_or_create_expect_same_instance_now_watched_Async);

        private async Task When_search_unwatched_channel_then_get_or_create_expect_same_instance_now_watched_Async()
        {
            var otherClient = await GetConnectedOtherClientAsync();
            var channel = await CreateUniqueTempChannelAsync(overrideClient: otherClient);
            var token = "promote-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var msg = await channel.SendNewMessageAsync(token);

            var response = await TryAsync(() => Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.EqualsTo(channel.Cid),
                },
                Query = token,
                WatchResultChannels = false,
            }), r => r != null && r.Results != null && r.Results.Any(x => x.Message != null && x.Message.Id == msg.Id));

            var unwatched = response.Results.First(r => r.Message.Id == msg.Id).Channel;
            Assert.IsFalse(unwatched.IsWatched);

            var watched = await Client.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);

            Assert.AreSame(unwatched, watched,
                "GetOrCreateChannelWithIdAsync should promote the existing search-cached instance, not create a new one.");
            Assert.IsTrue(watched.IsWatched, "After GetOrCreateChannelWithIdAsync the instance should be watched.");
            Assert.IsTrue(Client.WatchedChannels.Any(c => c.Cid == channel.Cid),
                "Promoted channel must now appear in WatchedChannels.");
        }

        // ---------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------

        private static void AssertOperator(KeyValuePair<string, object> entry, string expectedOperator,
            object expectedValue)
        {
            Assert.IsNotNull(entry.Value, "Filter entry value should not be null.");
            var dict = entry.Value as IDictionary<string, object>;
            Assert.IsNotNull(dict, "Filter entry value should serialize to a dictionary.");
            Assert.IsTrue(dict.ContainsKey(expectedOperator),
                "Expected operator '" + expectedOperator + "' not present. Got: " +
                string.Join(",", dict.Keys));
            Assert.AreEqual(expectedValue, dict[expectedOperator]);
        }

        private static async Task AssertThrowsAsync<TException>(Func<Task> action) where TException : Exception
        {
            try
            {
                await action();
            }
            catch (TException)
            {
                return;
            }
            catch (Exception e)
            {
                Assert.Fail("Expected " + typeof(TException).Name + " but caught " + e.GetType().Name +
                            ": " + e.Message);
                return;
            }

            Assert.Fail("Expected " + typeof(TException).Name + " but no exception was thrown.");
        }
    }
}
#endif
