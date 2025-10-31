#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.LowLevelClient.Models;
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
    }
}
#endif

