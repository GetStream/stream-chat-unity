using System;
using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Models
{
    /// <summary>
    /// Represents a poll vote
    /// </summary>
    public partial class PollVote : ILoadableFrom<PollVoteInternalDTO, PollVote>, ILoadableFrom<PollVoteResponseDataInternalDTO, PollVote>
    {
        public string AnswerText { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public string Id { get; set; }

        public bool? IsAnswer { get; set; }

        public string OptionId { get; set; }

        public string PollId { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }

        public Dictionary<string, object> AdditionalProperties { get; set; }

        PollVote ILoadableFrom<PollVoteInternalDTO, PollVote>.LoadFromDto(PollVoteInternalDTO dto)
        {
            AnswerText = dto.AnswerText;
            CreatedAt = dto.CreatedAt;
            Id = dto.Id;
            IsAnswer = dto.IsAnswer;
            OptionId = dto.OptionId;
            PollId = dto.PollId;
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            UserId = dto.UserId;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        PollVote ILoadableFrom<PollVoteResponseDataInternalDTO, PollVote>.LoadFromDto(PollVoteResponseDataInternalDTO dto)
        {
            AnswerText = dto.AnswerText;
            CreatedAt = dto.CreatedAt;
            Id = dto.Id;
            IsAnswer = dto.IsAnswer;
            OptionId = dto.OptionId;
            PollId = dto.PollId;
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserResponseInternalDTO, User>(dto.User);
            UserId = dto.UserId;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

