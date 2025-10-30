using System;
using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Models
{
    /// <summary>
    /// Represents a poll
    /// </summary>
    public partial class Poll : ModelBase, ILoadableFrom<PollInternalDTO, Poll>, ILoadableFrom<PollResponseDataInternalDTO, Poll>
    {
        public bool AllowAnswers { get; set; }

        public bool AllowUserSuggestedOptions { get; set; }

        public int AnswersCount { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public User CreatedBy { get; set; } // StreamTodo: check if we can replace with IStreamUser

        public string CreatedById { get; set; }

        public Dictionary<string, object> Custom { get; set; }

        public string Description { get; set; }

        public bool EnforceUniqueVote { get; set; }

        public string Id { get; set; }

        public bool? IsClosed { get; set; }

        public List<PollVote> LatestAnswers { get; set; }

        public Dictionary<string, List<PollVote>> LatestVotesByOption { get; set; }

        public int? MaxVotesAllowed { get; set; }

        public string Name { get; set; }

        public List<PollOption> Options { get; set; }

        public List<PollVote> OwnVotes { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public int VoteCount { get; set; }

        public Dictionary<string, int> VoteCountsByOption { get; set; }

        public VotingVisibility VotingVisibility { get; set; }

        Poll ILoadableFrom<PollInternalDTO, Poll>.LoadFromDto(PollInternalDTO dto)
        {
            AllowAnswers = dto.AllowAnswers;
            AllowUserSuggestedOptions = dto.AllowUserSuggestedOptions;
            AnswersCount = dto.AnswersCount;
            CreatedAt = dto.CreatedAt;
            CreatedBy = CreatedBy.TryLoadFromDto<UserObjectInternalDTO, User>(dto.CreatedBy);
            CreatedById = dto.CreatedById;
            Custom = dto.Custom;
            Description = dto.Description;
            EnforceUniqueVote = dto.EnforceUniqueVote;
            Id = dto.Id;
            IsClosed = dto.IsClosed;
            LatestAnswers = LatestAnswers.TryLoadFromDtoCollection(dto.LatestAnswers);
            LatestVotesByOption = LoadVotesByOption(dto.LatestVotesByOption);
            MaxVotesAllowed = dto.MaxVotesAllowed;
            Name = dto.Name;
            Options = Options.TryLoadFromDtoCollection(dto.Options);
            OwnVotes = OwnVotes.TryLoadFromDtoCollection(dto.OwnVotes);
            UpdatedAt = dto.UpdatedAt;
            VoteCount = dto.VoteCount;
            VoteCountsByOption = dto.VoteCountsByOption;
            VotingVisibility = dto.VotingVisibility;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        Poll ILoadableFrom<PollResponseDataInternalDTO, Poll>.LoadFromDto(PollResponseDataInternalDTO dto)
        {
            AllowAnswers = dto.AllowAnswers;
            AllowUserSuggestedOptions = dto.AllowUserSuggestedOptions;
            AnswersCount = dto.AnswersCount;
            CreatedAt = dto.CreatedAt;
            CreatedBy = CreatedBy.TryLoadFromDto<UserResponseInternalDTO, User>(dto.CreatedBy);
            CreatedById = dto.CreatedById;
            Custom = dto.Custom;
            Description = dto.Description;
            EnforceUniqueVote = dto.EnforceUniqueVote;
            Id = dto.Id;
            IsClosed = dto.IsClosed;
            LatestAnswers = LatestAnswers.TryLoadFromDtoCollection(dto.LatestAnswers);
            LatestVotesByOption = LoadVotesByOption(dto.LatestVotesByOption);
            MaxVotesAllowed = dto.MaxVotesAllowed;
            Name = dto.Name;
            Options = Options.TryLoadFromDtoCollection(dto.Options);
            OwnVotes = OwnVotes.TryLoadFromDtoCollection(dto.OwnVotes);
            UpdatedAt = dto.UpdatedAt;
            VoteCount = dto.VoteCount;
            VoteCountsByOption = dto.VoteCountsByOption;
            VotingVisibility = dto.VotingVisibility;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        private Dictionary<string, List<PollVote>> LoadVotesByOption(Dictionary<string, List<PollVoteResponseDataInternalDTO>> dto)
        {
            if (dto == null)
            {
                return null;
            }

            var result = new Dictionary<string, List<PollVote>>();
            foreach (var kvp in dto)
            {
                result[kvp.Key] = new List<PollVote>().TryLoadFromDtoCollection(kvp.Value);
            }
            return result;
        }
        
        private Dictionary<string, List<PollVote>> LoadVotesByOption(Dictionary<string, List<PollVoteInternalDTO>> dto)
        {
            if (dto == null)
            {
                return null;
            }

            var result = new Dictionary<string, List<PollVote>>();
            foreach (var kvp in dto)
            {
                result[kvp.Key] = new List<PollVote>().TryLoadFromDtoCollection(kvp.Value);
            }
            return result;
        }
    }
}

