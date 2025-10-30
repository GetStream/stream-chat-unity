using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Requests
{
    /// <summary>
    /// Request to create a poll
    /// </summary>
    public partial class CreatePollRequest : RequestObjectBase, ISavableTo<CreatePollRequestInternalDTO>
    {
        public Dictionary<string, object> Custom { get; set; }

        public bool? AllowAnswers { get; set; }

        public bool? AllowUserSuggestedOptions { get; set; }

        public string Description { get; set; }

        public bool? EnforceUniqueVote { get; set; }

        public string Id { get; set; }

        public bool? IsClosed { get; set; }

        public int? MaxVotesAllowed { get; set; }

        public string Name { get; set; }

        public List<PollOptionInput> Options { get; set; }

        public VotingVisibility? VotingVisibility { get; set; }

        CreatePollRequestInternalDTO ISavableTo<CreatePollRequestInternalDTO>.SaveToDto()
            => new CreatePollRequestInternalDTO
            {
                Custom = Custom,
                AllowAnswers = AllowAnswers,
                AllowUserSuggestedOptions = AllowUserSuggestedOptions,
                Description = Description,
                EnforceUniqueVote = EnforceUniqueVote,
                Id = Id,
                IsClosed = IsClosed,
                MaxVotesAllowed = MaxVotesAllowed,
                Name = Name,
                Options = Options.TrySaveToDtoCollection<PollOptionInput, PollOptionInputInternalDTO>(),
                VotingVisibility = VotingVisibility?.ToCreatePollRequestDto(),
            };
    }
}

