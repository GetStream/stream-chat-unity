using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Requests
{
    /// <summary>
    /// Request to cast a vote in a poll
    /// </summary>
    public partial class CastPollVoteRequest : RequestObjectBase, ISavableTo<CastPollVoteRequestInternalDTO>
    {
        public Vote Vote { get; set; }

        CastPollVoteRequestInternalDTO ISavableTo<CastPollVoteRequestInternalDTO>.SaveToDto()
            => new CastPollVoteRequestInternalDTO
            {
                Vote = Vote.TrySaveToDto<VoteDataInternalDTO>(),
            };
    }
}

