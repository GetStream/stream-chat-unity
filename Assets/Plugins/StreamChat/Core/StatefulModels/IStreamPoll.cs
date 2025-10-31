using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;

namespace StreamChat.Core.StatefulModels
{
    /// <summary>
    /// Poll event handler for poll state changes
    /// </summary>
    public delegate void StreamPollHandler(IStreamPoll poll);

    /// <summary>
    /// Poll vote event handler
    /// </summary>
    public delegate void StreamPollVoteHandler(IStreamPoll poll, StreamPollVote vote);

    /// <summary>
    /// Poll is a voting mechanism that can be attached to messages
    /// </summary>
    public interface IStreamPoll : IStreamStatefulModel
    {
        /// <summary>
        /// Event fired when this poll was closed
        /// </summary>
        event StreamPollHandler Closed;

        /// <summary>
        /// Event fired when this poll was updated
        /// </summary>
        event StreamPollHandler Updated;

        /// <summary>
        /// Event fired when a vote was cast on this poll
        /// </summary>
        event StreamPollVoteHandler VoteCasted;

        /// <summary>
        /// Event fired when a vote was changed on this poll
        /// </summary>
        event StreamPollVoteHandler VoteChanged;

        /// <summary>
        /// Event fired when a vote was removed from this poll
        /// </summary>
        event StreamPollVoteHandler VoteRemoved;

        /// <summary>
        /// Whether answers are allowed
        /// </summary>
        bool AllowAnswers { get; }

        /// <summary>
        /// Whether users can suggest their own options
        /// </summary>
        bool AllowUserSuggestedOptions { get; }

        /// <summary>
        /// The number of answers
        /// </summary>
        int AnswersCount { get; }

        /// <summary>
        /// Date/time of poll creation
        /// </summary>
        DateTimeOffset CreatedAt { get; }

        /// <summary>
        /// User who created the poll
        /// </summary>
        IStreamUser CreatedBy { get; }

        /// <summary>
        /// ID of the user who created the poll
        /// </summary>
        string CreatedById { get; }

        /// <summary>
        /// Poll description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Whether to enforce unique votes
        /// </summary>
        bool EnforceUniqueVote { get; }

        /// <summary>
        /// Poll unique ID
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Whether the poll is closed
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// Latest answers to the poll
        /// </summary>
        IReadOnlyList<StreamPollVote> LatestAnswers { get; }

        /// <summary>
        /// Latest votes grouped by option
        /// </summary>
        IReadOnlyDictionary<string, IReadOnlyList<StreamPollVote>> LatestVotesByOption { get; }

        /// <summary>
        /// Maximum number of votes allowed per user
        /// </summary>
        int? MaxVotesAllowed { get; }

        /// <summary>
        /// Poll name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// List of poll options
        /// </summary>
        IReadOnlyList<StreamPollOption> Options { get; }

        /// <summary>
        /// Votes cast by the local user
        /// </summary>
        IReadOnlyList<StreamPollVote> OwnVotes { get; }

        /// <summary>
        /// Date/time of last update
        /// </summary>
        DateTimeOffset UpdatedAt { get; }

        /// <summary>
        /// Total number of votes
        /// </summary>
        int VoteCount { get; }

        /// <summary>
        /// Number of votes per option
        /// </summary>
        IReadOnlyDictionary<string, int> VoteCountsByOption { get; }

        /// <summary>
        /// Voting visibility setting
        /// </summary>
        VotingVisibility VotingVisibility { get; }

        /// <summary>
        /// The channel this poll belongs to
        /// </summary>
        IStreamChannel Channel { get; }

        /// <summary>
        /// Cast a vote on this poll
        /// </summary>
        /// <param name="messageId">The ID of the message containing this poll</param>
        /// <param name="optionId">The ID of the option to vote for</param>
        Task<StreamPollVote> CastVoteAsync(string messageId, string optionId);

        /// <summary>
        /// Remove a vote from this poll
        /// </summary>
        /// <param name="messageId">The ID of the message containing this poll</param>
        /// <param name="voteId">The ID of the vote to remove</param>
        Task RemoveVoteAsync(string messageId, string voteId);

        /// <summary>
        /// Update this poll
        /// </summary>
        /// <param name="updateRequest">Update request with poll fields to change</param>
        Task UpdateAsync(StreamUpdatePollRequest updateRequest);

        /// <summary>
        /// Update poll in a partial mode. You can selectively set and unset fields of the poll
        /// If you want to completely overwrite the poll use the <see cref="UpdateAsync"/>
        /// </summary>
        /// <param name="setFields">Fields to set with new values</param>
        /// <param name="unsetFields">Fields to unset (remove)</param>
        Task UpdatePartialAsync(IDictionary<string, object> setFields = null, IEnumerable<string> unsetFields = null);

        /// <summary>
        /// Close this poll
        /// </summary>
        Task CloseAsync();

        /// <summary>
        /// Add an option to this poll
        /// </summary>
        /// <param name="text">The text of the new option</param>
        Task<StreamPollOption> AddOptionAsync(string text);

        /// <summary>
        /// Update an option on this poll
        /// </summary>
        /// <param name="optionId">The ID of the option to update</param>
        /// <param name="text">New text for the option</param>
        Task<StreamPollOption> UpdateOptionAsync(string optionId, string text);

        /// <summary>
        /// Delete an option from this poll
        /// </summary>
        /// <param name="optionId">The ID of the option to delete</param>
        Task DeleteOptionAsync(string optionId);
    }
}


