using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.State;

namespace StreamChat.Core.Requests
{
    /// <summary>
    /// Request to update a poll
    /// </summary>
    public class StreamUpdatePollRequest : ISavableTo<UpdatePollRequestInternalDTO>
    {
        /// <summary>
        /// Custom data for the poll
        /// </summary>
        public Dictionary<string, object> Custom { get; set; }

        /// <summary>
        /// Whether to allow answers
        /// </summary>
        public bool? AllowAnswers { get; set; }

        /// <summary>
        /// Whether to allow user suggested options
        /// </summary>
        public bool? AllowUserSuggestedOptions { get; set; }

        /// <summary>
        /// Poll description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether to enforce unique votes
        /// </summary>
        public bool? EnforceUniqueVote { get; set; }

        /// <summary>
        /// Whether the poll is closed
        /// </summary>
        public bool? IsClosed { get; set; }

        /// <summary>
        /// Maximum number of votes allowed per user
        /// </summary>
        public int? MaxVotesAllowed { get; set; }

        /// <summary>
        /// Poll name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Voting visibility setting
        /// </summary>
        public VotingVisibility? VotingVisibility { get; set; }

        UpdatePollRequestInternalDTO ISavableTo<UpdatePollRequestInternalDTO>.SaveToDto()
        {
            return new UpdatePollRequestInternalDTO
            {
                Custom = Custom,
                AllowAnswers = AllowAnswers,
                AllowUserSuggestedOptions = AllowUserSuggestedOptions,
                Description = Description,
                EnforceUniqueVote = EnforceUniqueVote,
                //Id = Id, Omitted, ID is set internally, not by a user
                IsClosed = IsClosed,
                MaxVotesAllowed = MaxVotesAllowed,
                Name = Name,
                VotingVisibility = VotingVisibility.HasValue ? (string)VotingVisibility.Value : null
            };
        }
    }
}

