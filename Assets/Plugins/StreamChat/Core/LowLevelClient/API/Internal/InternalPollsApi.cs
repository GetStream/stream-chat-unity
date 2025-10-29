using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    /// <summary>
    /// Internal API for polls operations
    /// </summary>
    internal class InternalPollsApi : InternalApiClientBase, IInternalPollsApi
    {
        public InternalPollsApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory, IStreamChatLowLevelClient lowLevelClient)
            : base(httpClient, serializer, logs, requestUriFactory, lowLevelClient)
        {
        }

        public Task<PollResponseInternalDTO> CreatePollAsync(CreatePollRequestInternalDTO createPollRequest)
            => Post<CreatePollRequestInternalDTO, PollResponseInternalDTO>("polls", createPollRequest);

        public Task<PollResponseInternalDTO> GetPollAsync(string pollId)
            => Get<PollResponseInternalDTO>($"polls/{pollId}");

        public Task<PollResponseInternalDTO> UpdatePollAsync(string pollId, UpdatePollRequestInternalDTO updatePollRequest)
            => Put<UpdatePollRequestInternalDTO, PollResponseInternalDTO>($"polls/{pollId}", updatePollRequest);

        public Task<PollResponseInternalDTO> UpdatePollPartialAsync(string pollId, UpdatePollPartialRequestInternalDTO updatePollPartialRequest)
            => Patch<UpdatePollPartialRequestInternalDTO, PollResponseInternalDTO>($"polls/{pollId}", updatePollPartialRequest);

        public Task<ResponseInternalDTO> DeletePollAsync(string pollId)
            => Delete<ResponseInternalDTO>($"polls/{pollId}");

        public Task<QueryPollsResponseInternalDTO> QueryPollsAsync(QueryPollsRequestInternalDTO queryPollsRequest)
            => Post<QueryPollsRequestInternalDTO, QueryPollsResponseInternalDTO>("polls/query", queryPollsRequest);

        public Task<PollOptionResponseInternalDTO> CreatePollOptionAsync(string pollId, CreatePollOptionRequestInternalDTO createPollOptionRequest)
            => Post<CreatePollOptionRequestInternalDTO, PollOptionResponseInternalDTO>($"polls/{pollId}/options", createPollOptionRequest);

        public Task<PollOptionResponseInternalDTO> GetPollOptionAsync(string pollId, string optionId)
            => Get<PollOptionResponseInternalDTO>($"polls/{pollId}/options/{optionId}");

        public Task<PollOptionResponseInternalDTO> UpdatePollOptionAsync(string pollId, string optionId, UpdatePollOptionRequestInternalDTO updatePollOptionRequest)
            => Put<UpdatePollOptionRequestInternalDTO, PollOptionResponseInternalDTO>($"polls/{pollId}/options/{optionId}", updatePollOptionRequest);

        public Task<ResponseInternalDTO> DeletePollOptionAsync(string pollId, string optionId)
            => Delete<ResponseInternalDTO>($"polls/{pollId}/options/{optionId}");

        public Task<PollVoteResponseInternalDTO> CastVoteAsync(string messageId, string pollId, CastPollVoteRequestInternalDTO castPollVoteRequest)
            => Post<CastPollVoteRequestInternalDTO, PollVoteResponseInternalDTO>($"messages/{messageId}/polls/{pollId}/vote", castPollVoteRequest);

        public Task<PollVoteResponseInternalDTO> RemoveVoteAsync(string messageId, string pollId, string voteId)
            => Delete<PollVoteResponseInternalDTO>($"messages/{messageId}/polls/{pollId}/vote/{voteId}");

        public Task<PollVotesResponseInternalDTO> QueryVotesAsync(string pollId, QueryPollVotesRequestInternalDTO queryPollVotesRequest)
            => Post<QueryPollVotesRequestInternalDTO, PollVotesResponseInternalDTO>($"polls/{pollId}/votes", queryPollVotesRequest);
    }
}

