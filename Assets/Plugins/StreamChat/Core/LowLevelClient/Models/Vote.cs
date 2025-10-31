using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    /// <summary>
    /// Represents vote data for casting a vote
    /// </summary>
    public partial class Vote : ISavableTo<VoteDataInternalDTO>
    {
        public string AnswerText { get; set; }

        public string OptionId { get; set; }

        VoteDataInternalDTO ISavableTo<VoteDataInternalDTO>.SaveToDto()
            => new VoteDataInternalDTO
            {
                AnswerText = AnswerText,
                OptionId = OptionId,
            };
    }
}

