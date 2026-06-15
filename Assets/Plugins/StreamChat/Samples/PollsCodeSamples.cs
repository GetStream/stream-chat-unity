using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Polls;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Samples
{
    internal sealed class PollsCodeSamples
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#polls-at-a-quick-glance
        /// </summary>
        public async Task QuickGlance()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            // Create a poll and send it in a message
            var poll = await Client.Polls.CreatePollAsync(new StreamCreatePollRequest
            {
                Name = "Where should we host our next event?",
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "Amsterdam" },
                    new StreamPollOptionRequest { Text = "Boulder" },
                },
            });
            var message = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = "Vote now!",
                PollId = poll.Id,
            });

            // Vote on a poll
            var optionId = poll.Options[0].Id;
            await poll.CastVoteAsync(message.Id, optionId);

            // Retrieve poll results
            var refreshed = await Client.Polls.GetPollAsync(poll.Id);
            var voteCountsByOption = refreshed.VoteCountsByOption; // { 'option-id': 5 }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#creating-a-poll-and-sending-it-as-part-of-a-message
        /// </summary>
        public async Task CreatePollWithMessage()
        {
            // Create a poll with options
            var createPollRequest = new StreamCreatePollRequest
            {
                Name = "Where should we host our next company event?",
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest { Text = "Amsterdam, The Netherlands" },
                    new StreamPollOptionRequest { Text = "Boulder, CO" }
                }
            };

            var poll = await Client.Polls.CreatePollAsync(createPollRequest);

            // Get or create a channel
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            // Send a message with the poll
            var messageRequest = new StreamSendMessageRequest
            {
                Text = "We want to know your opinion!",
                PollId = poll.Id
            };

            var message = await channel.SendNewMessageAsync(messageRequest);

            // message.PollId contains the poll ID
            // Others users can use poll ID to fetch the poll object
            var pollData = await Client.Polls.GetPollAsync(message.PollId);

            // The poll object contains data, events, and operations:
            // - Data: pollData.Name, pollData.Options, pollData.VoteCount, pollData.IsClosed, etc.
            // - Events: pollData.Updated, pollData.Closed, pollData.VoteCasted, etc.
            // - Operations: pollData.CastVoteAsync(), pollData.CloseAsync(), pollData.UpdateAsync(), etc.
            // Explore the IStreamPoll interface for complete reference
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#poll-options
        /// Custom properties on a poll and on individual options.
        /// </summary>
        public async Task CreatePollWithCustomData()
        {
            var createPollRequest = new StreamCreatePollRequest
            {
                Name = "Where should we host our next company event?",
                Options = new List<StreamPollOptionRequest>
                {
                    new StreamPollOptionRequest
                    {
                        Text = "Amsterdam, The Netherlands",
                        Custom = new Dictionary<string, object>
                        {
                            { "venue_capacity", 300 }
                        }
                    },
                    new StreamPollOptionRequest
                    {
                        Text = "Boulder, CO",
                        Custom = new Dictionary<string, object>
                        {
                            { "venue_capacity", 1000 }
                        }
                    }
                },
                Custom = new Dictionary<string, object>
                {
                    { "category", "company-events" },
                    { "priority", 5 }
                }
            };

            var poll = await Client.Polls.CreatePollAsync(createPollRequest);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#send-vote-on-option
        /// </summary>
        public async Task CastVoteOnOption()
        {
            // Get the channel and message
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            // Assume we have a message with a poll
            var message = channel.Messages.First(m => !string.IsNullOrEmpty(m.PollId));
            var poll = await Client.Polls.GetPollAsync(message.PollId);

            // Get an option to vote on
            var option = poll.Options[0];

            // Cast a vote on the option
            await poll.CastVoteAsync(message.Id, option.Id);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#send-an-answer-(if-answers-are-configured-to-be-allowed)
        /// Poll answers are not yet exposed by the Unity SDK; the docs show the
        /// "raise an issue" placeholder. Tracked in F11 of the docs update plan.
        /// </summary>
        public Task CastAnswer()
        {
            // Poll answers are not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            return Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#removing-a-vote
        /// </summary>
        public async Task RemoveVote()
        {
            // Get the channel and message
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            // Assume we have a message with a poll
            var message = channel.Messages.First(m => !string.IsNullOrEmpty(m.PollId));
            var poll = await Client.Polls.GetPollAsync(message.PollId);

            // Get a vote to remove (e.g., from own votes)
            var vote = poll.OwnVotes[0];

            // Remove the vote
            await poll.RemoveVoteAsync(message.Id, vote.Id);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#closing-a-poll
        /// </summary>
        public async Task ClosePoll()
        {
            // Get the poll reference
            var poll = await Client.Polls.GetPollAsync("poll-id");

            // Close the poll to prevent further votes
            await poll.CloseAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#retrieving-a-poll
        /// </summary>
        public async Task RetrievePoll()
        {
            // Retrieve a poll by ID
            var poll = await Client.Polls.GetPollAsync("poll-id");

            // Access poll properties
            var name = poll.Name;
            var description = poll.Description;
            var options = poll.Options;
            var voteCount = poll.VoteCount;
            var isClosed = poll.IsClosed;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#full-update
        /// </summary>
        public async Task UpdatePollFull()
        {
            // Get the poll reference
            var poll = await Client.Polls.GetPollAsync("poll-id");

            // Update the poll (full update)
            var updateRequest = new StreamUpdatePollRequest
            {
                Name = "Where should we not go to?",
                Description = "Updated description"
            };

            await poll.UpdateAsync(updateRequest);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#partial-update
        /// </summary>
        public async Task UpdatePollPartial()
        {
            // Get the poll reference
            var poll = await Client.Polls.GetPollAsync("poll-id");

            // Partial update - set specific fields and/or unset fields
            await poll.UpdatePartialAsync(
                setFields: new Dictionary<string, object>
                {
                    { "name", "Where should we not go to?" },
                    { "max_votes_allowed", 3 }
                },
                unsetFields: new List<string> { "custom_property" });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#deleting-a-poll
        /// </summary>
        public async Task DeletePoll()
        {
            // Delete a poll by ID through the Polls API
            await Client.Polls.DeletePollAsync("poll-id");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#add-poll-option
        /// </summary>
        public async Task AddPollOption()
        {
            // Get the poll reference
            var poll = await Client.Polls.GetPollAsync("poll-id");

            // Add a new option - takes a text string parameter
            var newOption = await poll.AddOptionAsync("Another option");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#update-poll-option
        /// </summary>
        public async Task UpdatePollOption()
        {
            // Get the poll reference
            var poll = await Client.Polls.GetPollAsync("poll-id");

            // Get an option to update
            var option = poll.Options[0];

            // Update the option - takes optionId and text parameters
            var updatedOption = await poll.UpdateOptionAsync(option.Id, "Updated option");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#delete-poll-option
        /// </summary>
        public async Task DeletePollOption()
        {
            // Get the poll reference
            var poll = await Client.Polls.GetPollAsync("poll-id");

            // Get an option to delete
            var option = poll.Options[0];

            // Delete the option
            await poll.DeleteOptionAsync(option.Id);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#querying-votes
        /// Querying poll votes is not yet exposed by the Unity SDK. The docs
        /// surface this placeholder; tracked in F11 of the docs update plan.
        /// </summary>
        public Task QueryPollVotes()
        {
            // Querying poll votes is not yet available in the Unity SDK.
            // You can access latest votes through poll.LatestVotesByOption and poll.OwnVotes properties.
            // Please let us know if you'd like full vote querying implemented: https://github.com/GetStream/stream-chat-unity/issues
            return Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/polls-api/?language=unity#querying-polls
        /// </summary>
        public async Task QueryPolls()
        {
            // Retrieve all polls that are closed for voting, sorted by created_at
            var queryRequest = new StreamQueryPollsRequest
            {
                Filter = new IFieldFilterRule[]
                {
                    PollFilter.IsClosed.EqualsTo(true)
                },
                Sort = PollSort.OrderByDescending(PollSortFieldName.CreatedAt)
            };

            var polls = await Client.Polls.QueryPollsAsync(queryRequest);
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}
