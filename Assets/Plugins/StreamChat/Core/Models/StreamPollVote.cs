using System;
using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Models
{
    /// <summary>
    /// Represents a vote cast on a poll
    /// </summary>
    public class StreamPollVote : IStateLoadableFrom<PollVoteInternalDTO, StreamPollVote>, 
        IStateLoadableFrom<PollVoteResponseDataInternalDTO, StreamPollVote>
    {
        /// <summary>
        /// Text answer for the vote (if poll allows answers)
        /// </summary>
        public string AnswerText { get; private set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public DateTimeOffset? CreatedAt { get; private set; }

        /// <summary>
        /// Unique vote ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Whether this is an answer (vs a vote for an option)
        /// </summary>
        public bool? IsAnswer { get; private set; }

        /// <summary>
        /// ID of the option that was voted for
        /// </summary>
        public string OptionId { get; private set; }

        /// <summary>
        /// ID of the poll this vote belongs to
        /// </summary>
        public string PollId { get; private set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; private set; }

        /// <summary>
        /// User who cast the vote
        /// </summary>
        public IStreamUser User { get; private set; }

        /// <summary>
        /// ID of the user who cast the vote
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Additional custom properties
        /// </summary>
        public IReadOnlyDictionary<string, object> AdditionalProperties { get; private set; }

        StreamPollVote IStateLoadableFrom<PollVoteInternalDTO, StreamPollVote>.LoadFromDto(PollVoteInternalDTO dto, ICache cache)
        {
            AnswerText = dto.AnswerText;
            CreatedAt = dto.CreatedAt;
            Id = dto.Id;
            IsAnswer = dto.IsAnswer;
            OptionId = dto.OptionId;
            PollId = dto.PollId;
            UpdatedAt = dto.UpdatedAt;
            User = cache.TryCreateOrUpdate(dto.User);
            UserId = dto.UserId;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        StreamPollVote IStateLoadableFrom<PollVoteResponseDataInternalDTO, StreamPollVote>.LoadFromDto(PollVoteResponseDataInternalDTO dto, ICache cache)
        {
            AnswerText = dto.AnswerText;
            CreatedAt = dto.CreatedAt;
            Id = dto.Id;
            IsAnswer = dto.IsAnswer;
            OptionId = dto.OptionId;
            PollId = dto.PollId;
            UpdatedAt = dto.UpdatedAt;
            User = cache.TryCreateOrUpdate(dto.User);
            UserId = dto.UserId;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

