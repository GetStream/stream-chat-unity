#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Polls;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests operations for Polls API
    /// </summary>
    internal class PollsTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_creating_poll_with_options_expect_poll_created()
            => ConnectAndExecute(When_creating_poll_with_options_expect_poll_created_Async);

        private async Task When_creating_poll_with_options_expect_poll_created_Async()
        {
            var pollId = "poll-" + Guid.NewGuid();

            var createPollRequest = new StreamCreatePollRequest
            {
                Id = pollId,
                Name = "What is your favorite programming language?",
                Description = "Let us know which language you prefer",
                EnforceUniqueVote = true,
                AllowAnswers = false,
                AllowUserSuggestedOptions = false,
                MaxVotesAllowed = 1,
                VotingVisibility = VotingVisibility.Public,
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "C#" },
                    new StreamPollOptionRequest { Text = "JavaScript" },
                    new StreamPollOptionRequest { Text = "Python" },
                    new StreamPollOptionRequest { Text = "Go" }
                }
            };

            var poll = await Client.Polls.CreatePollAsync(createPollRequest);

            Assert.NotNull(poll);
            Assert.AreEqual(pollId, poll.Id);
            Assert.AreEqual("What is your favorite programming language?", poll.Name);
            Assert.AreEqual("Let us know which language you prefer", poll.Description);
            Assert.AreEqual(true, poll.EnforceUniqueVote);
            Assert.AreEqual(false, poll.AllowAnswers);
            Assert.AreEqual(false, poll.AllowUserSuggestedOptions);
            Assert.AreEqual(1, poll.MaxVotesAllowed);
            Assert.AreEqual(VotingVisibility.Public, poll.VotingVisibility);
            
            Assert.NotNull(poll.Options);
            Assert.AreEqual(4, poll.Options.Count);
            
            var optionTexts = poll.Options.Select(o => o.Text).ToList();
            Assert.Contains("C#", optionTexts);
            Assert.Contains("JavaScript", optionTexts);
            Assert.Contains("Python", optionTexts);
            Assert.Contains("Go", optionTexts);
        }

        [UnityTest]
        public IEnumerator When_fetching_poll_expect_poll_returned()
            => ConnectAndExecute(When_fetching_poll_expect_poll_returned_Async);

        private async Task When_fetching_poll_expect_poll_returned_Async()
        {
            var pollId = "poll-" + Guid.NewGuid();
            var pollName = "Best IDE for Unity development?";

            // First, create a poll
            var createPollRequest = new StreamCreatePollRequest
            {
                Id = pollId,
                Name = pollName,
                Description = "Help us understand your preferences",
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "Visual Studio" },
                    new StreamPollOptionRequest { Text = "Rider" },
                    new StreamPollOptionRequest { Text = "VS Code" }
                }
            };

            var createdPoll = await Client.Polls.CreatePollAsync(createPollRequest);
            Assert.NotNull(createdPoll);

            // Now fetch the poll
            var fetchedPoll = await Client.Polls.GetPollAsync(pollId);

            Assert.NotNull(fetchedPoll);
            Assert.AreEqual(pollId, fetchedPoll.Id);
            Assert.AreEqual(pollName, fetchedPoll.Name);
            Assert.AreEqual(3, fetchedPoll.Options.Count);
            
            var optionTexts = fetchedPoll.Options.Select(o => o.Text).ToList();
            Assert.Contains("Visual Studio", optionTexts);
            Assert.Contains("Rider", optionTexts);
            Assert.Contains("VS Code", optionTexts);
        }

        [UnityTest]
        public IEnumerator When_sending_message_with_poll_expect_poll_in_message()
            => ConnectAndExecute(When_sending_message_with_poll_expect_poll_in_message_Async);

        private async Task When_sending_message_with_poll_expect_poll_in_message_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var pollId = "poll-" + Guid.NewGuid();

            // Create a poll
            var createPollRequest = new StreamCreatePollRequest
            {
                Id = pollId,
                Name = "Which feature should we prioritize?",
                Description = "Vote for the most important feature",
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "Performance improvements" },
                    new StreamPollOptionRequest { Text = "New UI components" },
                    new StreamPollOptionRequest { Text = "Better documentation" },
                    new StreamPollOptionRequest { Text = "More examples" }
                }
            };

            var poll = await Client.Polls.CreatePollAsync(createPollRequest);
            Assert.NotNull(poll);

            // Send a message with the poll
            var messageRequest = new StreamSendMessageRequest
            {
                Text = "Please vote on our poll!",
                PollId = pollId
            };

            var message = await channel.SendNewMessageAsync(messageRequest);

            Assert.NotNull(message);
            Assert.AreEqual("Please vote on our poll!", message.Text);
            Assert.AreEqual(pollId, message.PollId);

            // Verify the poll can be fetched using the poll ID from the message
            var fetchedPoll = await Client.Polls.GetPollAsync(message.PollId);
            Assert.NotNull(fetchedPoll);
            Assert.AreEqual(pollId, fetchedPoll.Id);
            Assert.AreEqual("Which feature should we prioritize?", fetchedPoll.Name);
        }

        [UnityTest]
        public IEnumerator When_creating_poll_with_custom_data_expect_custom_data_preserved()
            => ConnectAndExecute(When_creating_poll_with_custom_data_expect_custom_data_preserved_Async);

        private async Task When_creating_poll_with_custom_data_expect_custom_data_preserved_Async()
        {
            var pollId = "poll-" + Guid.NewGuid();

            var createPollRequest = new StreamCreatePollRequest
            {
                Id = pollId,
                Name = "Test Poll",
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "Option 1" },
                    new StreamPollOptionRequest { Text = "Option 2" }
                },
                Custom = new Dictionary<string, object>
                {
                    { "category", "testing" },
                    { "priority", 5 },
                    { "tags", new List<string> { "unit-test", "polls" } }
                }
            };

            var poll = await Client.Polls.CreatePollAsync(createPollRequest);

            Assert.NotNull(poll);
            Assert.NotNull(poll.CustomData);
            Assert.IsTrue(poll.CustomData.ContainsKey("category"));
            Assert.AreEqual("testing", poll.CustomData.Get<string>("category"));
            Assert.IsTrue(poll.CustomData.ContainsKey("priority"));
        }

        [UnityTest]
        public IEnumerator When_updating_poll_expect_poll_updated()
            => ConnectAndExecute(When_updating_poll_expect_poll_updated_Async);

        private async Task When_updating_poll_expect_poll_updated_Async()
        {
            var pollId = "poll-" + Guid.NewGuid();

            // Create a poll
            var createPollRequest = new StreamCreatePollRequest
            {
                Id = pollId,
                Name = "Original Name",
                Description = "Original Description",
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "Option 1" }
                }
            };

            var poll = await Client.Polls.CreatePollAsync(createPollRequest);
            Assert.NotNull(poll);

            // Update the poll
            var updatePollRequest = new StreamUpdatePollRequest
            {
                Name = "Updated Name",
                Description = "Updated Description"
            };

            await poll.UpdateAsync(updatePollRequest);

            Assert.AreEqual(pollId, poll.Id);
            Assert.AreEqual("Updated Name", poll.Name);
            Assert.AreEqual("Updated Description", poll.Description);
        }

        [UnityTest]
        public IEnumerator When_closing_poll_expect_poll_closed()
            => ConnectAndExecute(When_closing_poll_expect_poll_closed_Async);

        private async Task When_closing_poll_expect_poll_closed_Async()
        {
            var pollId = "poll-" + Guid.NewGuid();

            // Create a poll
            var createPollRequest = new StreamCreatePollRequest
            {
                Id = pollId,
                Name = "Test Poll",
                IsClosed = false,
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "Option 1" }
                }
            };

            var poll = await Client.Polls.CreatePollAsync(createPollRequest);
            Assert.NotNull(poll);
            Assert.AreEqual(false, poll.IsClosed);

            // Close the poll
            await poll.CloseAsync();

            await WaitWhileFalseAsync(() => poll.IsClosed);

            Assert.AreEqual(true, poll.IsClosed);
        }

        [UnityTest]
        public IEnumerator When_adding_poll_option_expect_option_added()
            => ConnectAndExecute(When_adding_poll_option_expect_option_added_Async);

        private async Task When_adding_poll_option_expect_option_added_Async()
        {
            var pollId = "poll-" + Guid.NewGuid();

            // Create a poll with initial options
            var createPollRequest = new StreamCreatePollRequest
            {
                Id = pollId,
                Name = "Test Poll",
                AllowUserSuggestedOptions = true,
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "Option 1" },
                    new StreamPollOptionRequest { Text = "Option 2" }
                }
            };

            var poll = await Client.Polls.CreatePollAsync(createPollRequest);
            Assert.NotNull(poll);
            Assert.AreEqual(2, poll.Options.Count);

            // Add a new option
            var newOption = await poll.AddOptionAsync("Option 3");

            //await WaitWhileFalseAsync(() => poll.Options.Count == 3);

            Assert.NotNull(newOption);
            Assert.AreEqual("Option 3", newOption.Text);

            // Verify the poll now has 3 options (state should be auto-updated)
            Assert.AreEqual(3, poll.Options.Count);
        }

        [UnityTest]
        public IEnumerator When_querying_polls_with_filters_expect_filtered_results()
            => ConnectAndExecute(When_querying_polls_with_filters_expect_filtered_results_Async);

        private async Task When_querying_polls_with_filters_expect_filtered_results_Async()
        {
            // Create multiple polls with different properties
            var poll1Id = "poll-query-test-" + Guid.NewGuid();
            var poll2Id = "poll-query-test-" + Guid.NewGuid();
            var poll3Id = "poll-query-test-" + Guid.NewGuid();
            var poll4Id = "poll-query-test-" + Guid.NewGuid();

            var poll1 = await Client.Polls.CreatePollAsync(new StreamCreatePollRequest
            {
                Id = poll1Id,
                Name = "Programming Languages Poll",
                Description = "Vote for your favorite",
                VotingVisibility = VotingVisibility.Public,
                MaxVotesAllowed = 1,
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "C#" },
                    new StreamPollOptionRequest { Text = "JavaScript" }
                }
            });

            var poll2 = await Client.Polls.CreatePollAsync(new StreamCreatePollRequest
            {
                Id = poll2Id,
                Name = "IDE Preferences",
                Description = "Which IDE do you use?",
                VotingVisibility = VotingVisibility.Anonymous,
                MaxVotesAllowed = 2,
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "Visual Studio" },
                    new StreamPollOptionRequest { Text = "Rider" }
                }
            });

            var poll3 = await Client.Polls.CreatePollAsync(new StreamCreatePollRequest
            {
                Id = poll3Id,
                Name = "Framework Poll",
                Description = "Best framework?",
                VotingVisibility = VotingVisibility.Public,
                MaxVotesAllowed = 1,
                IsClosed = true,
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "Unity" },
                    new StreamPollOptionRequest { Text = "Unreal" }
                }
            });

            var poll4 = await Client.Polls.CreatePollAsync(new StreamCreatePollRequest
            {
                Id = poll4Id,
                Name = "Testing Poll",
                Description = "Just a test",
                VotingVisibility = VotingVisibility.Public,
                MaxVotesAllowed = 3,
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "Option 1" }
                }
            });

            // Test 1: Query specific polls by ID
            var queryRequest1 = new StreamQueryPollsRequest
            {
                Filter = new IFieldFilterRule[]
                {
                    PollFilter.Id.In(poll1Id, poll2Id, poll3Id)
                },
                Sort = PollSort.OrderByDescending(PollSortFieldName.CreatedAt),
                Limit = 10
            };

            var result1 = (await Client.Polls.QueryPollsAsync(queryRequest1)).ToList();

            Assert.NotNull(result1);
            Assert.AreEqual(3, result1.Count);
            Assert.IsTrue(result1.Any(p => p.Id == poll1Id));
            Assert.IsTrue(result1.Any(p => p.Id == poll2Id));
            Assert.IsTrue(result1.Any(p => p.Id == poll3Id));
            Assert.IsFalse(result1.Any(p => p.Id == poll4Id));

            // Test 2: Query polls with name filter
            var queryRequest2 = new StreamQueryPollsRequest
            {
                Filter = new IFieldFilterRule[]
                {
                    PollFilter.Name.EqualsTo("Programming Languages Poll")
                },
                Limit = 10
            };

            var result2 = (await Client.Polls.QueryPollsAsync(queryRequest2)).ToList();

            Assert.NotNull(result2);
            Assert.AreEqual(1, result2.Count);
            Assert.AreEqual(poll1Id, result2[0].Id);
            Assert.AreEqual("Programming Languages Poll", result2[0].Name);

            // Test 3: Query only open polls (not closed)
            var queryRequest3 = new StreamQueryPollsRequest
            {
                Filter = new IFieldFilterRule[]
                {
                    PollFilter.Id.In(poll1Id, poll2Id, poll3Id, poll4Id),
                    PollFilter.IsClosed.EqualsTo(false)
                },
                Sort = PollSort.OrderByAscending(PollSortFieldName.Name),
                Limit = 10
            };

            var result3 = (await Client.Polls.QueryPollsAsync(queryRequest3)).ToList();

            Assert.NotNull(result3);
            Assert.AreEqual(3, result3.Count);
            Assert.IsTrue(result3.Any(p => p.Id == poll1Id));
            Assert.IsTrue(result3.Any(p => p.Id == poll2Id));
            Assert.IsTrue(result3.Any(p => p.Id == poll4Id));
            Assert.IsFalse(result3.Any(p => p.Id == poll3Id)); // poll3 is closed

            // Test 4: Query with pagination
            var queryRequest4 = new StreamQueryPollsRequest
            {
                Filter = new IFieldFilterRule[]
                {
                    PollFilter.Id.In(poll1Id, poll2Id, poll3Id, poll4Id)
                },
                Sort = PollSort.OrderByDescending(PollSortFieldName.CreatedAt),
                Limit = 2
            };

            var result4 = (await Client.Polls.QueryPollsAsync(queryRequest4)).ToList();

            Assert.NotNull(result4);
            Assert.AreEqual(2, result4.Count);
            // Should get the 2 most recently created polls
            Assert.AreEqual(poll4Id, result4[0].Id);
            Assert.AreEqual(poll3Id, result4[1].Id);
        }
    }
}
#endif

