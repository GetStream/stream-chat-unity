#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
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

            var createPollRequest = new CreatePollRequest
            {
                Id = pollId,
                Name = "What is your favorite programming language?",
                Description = "Let us know which language you prefer",
                EnforceUniqueVote = true,
                AllowAnswers = false,
                AllowUserSuggestedOptions = false,
                MaxVotesAllowed = 1,
                VotingVisibility = VotingVisibility.Public,
                Options = new List<PollOptionInput>
                {
                    new PollOptionInput { Text = "C#" },
                    new PollOptionInput { Text = "JavaScript" },
                    new PollOptionInput { Text = "Python" },
                    new PollOptionInput { Text = "Go" }
                }
            };

            var response = await Client.InternalLowLevelClient.PollsApi.CreatePollAsync(createPollRequest);

            Assert.NotNull(response);
            Assert.NotNull(response.Poll);
            Assert.AreEqual(pollId, response.Poll.Id);
            Assert.AreEqual("What is your favorite programming language?", response.Poll.Name);
            Assert.AreEqual("Let us know which language you prefer", response.Poll.Description);
            Assert.AreEqual(true, response.Poll.EnforceUniqueVote);
            Assert.AreEqual(false, response.Poll.AllowAnswers);
            Assert.AreEqual(false, response.Poll.AllowUserSuggestedOptions);
            Assert.AreEqual(1, response.Poll.MaxVotesAllowed);
            Assert.AreEqual(VotingVisibility.Public, response.Poll.VotingVisibility);
            
            Assert.NotNull(response.Poll.Options);
            Assert.AreEqual(4, response.Poll.Options.Count);
            
            var optionTexts = response.Poll.Options.Select(o => o.Text).ToList();
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
            var createPollRequest = new CreatePollRequest
            {
                Id = pollId,
                Name = pollName,
                Description = "Help us understand your preferences",
                Options = new List<PollOptionInput>
                {
                    new PollOptionInput { Text = "Visual Studio" },
                    new PollOptionInput { Text = "Rider" },
                    new PollOptionInput { Text = "VS Code" }
                }
            };

            var createResponse = await Client.InternalLowLevelClient.PollsApi.CreatePollAsync(createPollRequest);
            Assert.NotNull(createResponse);
            Assert.NotNull(createResponse.Poll);

            // Now fetch the poll
            var fetchResponse = await Client.InternalLowLevelClient.PollsApi.GetPollAsync(pollId);

            Assert.NotNull(fetchResponse);
            Assert.NotNull(fetchResponse.Poll);
            Assert.AreEqual(pollId, fetchResponse.Poll.Id);
            Assert.AreEqual(pollName, fetchResponse.Poll.Name);
            Assert.AreEqual(3, fetchResponse.Poll.Options.Count);
            
            var optionTexts = fetchResponse.Poll.Options.Select(o => o.Text).ToList();
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
            var createPollRequest = new CreatePollRequest
            {
                Id = pollId,
                Name = "Which feature should we prioritize?",
                Description = "Vote for the most important feature",
                Options = new List<PollOptionInput>
                {
                    new PollOptionInput { Text = "Performance improvements" },
                    new PollOptionInput { Text = "New UI components" },
                    new PollOptionInput { Text = "Better documentation" },
                    new PollOptionInput { Text = "More examples" }
                }
            };

            var pollResponse = await Client.InternalLowLevelClient.PollsApi.CreatePollAsync(createPollRequest);
            Assert.NotNull(pollResponse);
            Assert.NotNull(pollResponse.Poll);

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
            var fetchedPoll = await Client.InternalLowLevelClient.PollsApi.GetPollAsync(message.PollId);
            Assert.NotNull(fetchedPoll);
            Assert.NotNull(fetchedPoll.Poll);
            Assert.AreEqual(pollId, fetchedPoll.Poll.Id);
            Assert.AreEqual("Which feature should we prioritize?", fetchedPoll.Poll.Name);
        }

        [UnityTest]
        public IEnumerator When_creating_poll_with_custom_data_expect_custom_data_preserved()
            => ConnectAndExecute(When_creating_poll_with_custom_data_expect_custom_data_preserved_Async);

        private async Task When_creating_poll_with_custom_data_expect_custom_data_preserved_Async()
        {
            var pollId = "poll-" + Guid.NewGuid();

            var createPollRequest = new CreatePollRequest
            {
                Id = pollId,
                Name = "Test Poll",
                Options = new List<PollOptionInput>
                {
                    new PollOptionInput { Text = "Option 1" },
                    new PollOptionInput { Text = "Option 2" }
                },
                Custom = new Dictionary<string, object>
                {
                    { "category", "testing" },
                    { "priority", 5 },
                    { "tags", new List<string> { "unit-test", "polls" } }
                }
            };

            var response = await Client.InternalLowLevelClient.PollsApi.CreatePollAsync(createPollRequest);

            Assert.NotNull(response);
            Assert.NotNull(response.Poll);
            Assert.NotNull(response.Poll.Custom);
            Assert.IsTrue(response.Poll.Custom.ContainsKey("category"));
            Assert.AreEqual("testing", response.Poll.Custom["category"]);
            Assert.IsTrue(response.Poll.Custom.ContainsKey("priority"));
        }

        [UnityTest]
        public IEnumerator When_updating_poll_expect_poll_updated()
            => ConnectAndExecute(When_updating_poll_expect_poll_updated_Async);

        private async Task When_updating_poll_expect_poll_updated_Async()
        {
            var pollId = "poll-" + Guid.NewGuid();

            // Create a poll
            var createPollRequest = new CreatePollRequest
            {
                Id = pollId,
                Name = "Original Name",
                Description = "Original Description",
                Options = new List<PollOptionInput>
                {
                    new PollOptionInput { Text = "Option 1" }
                }
            };

            var createResponse = await Client.InternalLowLevelClient.PollsApi.CreatePollAsync(createPollRequest);
            Assert.NotNull(createResponse);

            // Update the poll
            var updatePollRequest = new UpdatePollRequest
            {
                Name = "Updated Name",
                Description = "Updated Description"
            };

            var updateResponse = await Client.InternalLowLevelClient.PollsApi.UpdatePollAsync(pollId, updatePollRequest);

            Assert.NotNull(updateResponse);
            Assert.NotNull(updateResponse.Poll);
            Assert.AreEqual(pollId, updateResponse.Poll.Id);
            Assert.AreEqual("Updated Name", updateResponse.Poll.Name);
            Assert.AreEqual("Updated Description", updateResponse.Poll.Description);
        }

        [UnityTest]
        public IEnumerator When_closing_poll_expect_poll_closed()
            => ConnectAndExecute(When_closing_poll_expect_poll_closed_Async);

        private async Task When_closing_poll_expect_poll_closed_Async()
        {
            var pollId = "poll-" + Guid.NewGuid();

            // Create a poll
            var createPollRequest = new CreatePollRequest
            {
                Id = pollId,
                Name = "Test Poll",
                IsClosed = false,
                Options = new List<PollOptionInput>
                {
                    new PollOptionInput { Text = "Option 1" }
                }
            };

            var createResponse = await Client.InternalLowLevelClient.PollsApi.CreatePollAsync(createPollRequest);
            Assert.NotNull(createResponse);
            Assert.AreEqual(false, createResponse.Poll.IsClosed);

            // Close the poll using partial update
            var updatePartialRequest = new UpdatePollPartialRequest
            {
                Set = new Dictionary<string, object>
                {
                    { "is_closed", true }
                }
            };

            var updateResponse = await Client.InternalLowLevelClient.PollsApi.UpdatePollPartialAsync(pollId, updatePartialRequest);

            Assert.NotNull(updateResponse);
            Assert.NotNull(updateResponse.Poll);
            Assert.AreEqual(true, updateResponse.Poll.IsClosed);
        }

        [UnityTest]
        public IEnumerator When_adding_poll_option_expect_option_added()
            => ConnectAndExecute(When_adding_poll_option_expect_option_added_Async);

        private async Task When_adding_poll_option_expect_option_added_Async()
        {
            var pollId = "poll-" + Guid.NewGuid();

            // Create a poll with initial options
            var createPollRequest = new CreatePollRequest
            {
                Id = pollId,
                Name = "Test Poll",
                AllowUserSuggestedOptions = true,
                Options = new List<PollOptionInput>
                {
                    new PollOptionInput { Text = "Option 1" },
                    new PollOptionInput { Text = "Option 2" }
                }
            };

            var createResponse = await Client.InternalLowLevelClient.PollsApi.CreatePollAsync(createPollRequest);
            Assert.NotNull(createResponse);
            Assert.AreEqual(2, createResponse.Poll.Options.Count);

            // Add a new option
            var createOptionRequest = new CreatePollOptionRequest
            {
                Text = "Option 3"
            };

            var optionResponse = await Client.InternalLowLevelClient.PollsApi.CreatePollOptionAsync(pollId, createOptionRequest);

            Assert.NotNull(optionResponse);
            Assert.NotNull(optionResponse.PollOption);
            Assert.AreEqual("Option 3", optionResponse.PollOption.Text);

            // Verify the poll now has 3 options
            var fetchResponse = await Client.InternalLowLevelClient.PollsApi.GetPollAsync(pollId);
            Assert.AreEqual(3, fetchResponse.Poll.Options.Count);
        }
    }
}
#endif

