using System;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.API.Internal;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;

namespace StreamChat.Core.LowLevelClient.API
{
    /// <summary>
    /// API client for polls operations
    /// </summary>
    internal class PollsApi : IPollsApi
    {
        public PollsApi(IInternalPollsApi internalPollsApi)
        {
            _internalPollsApi = internalPollsApi ?? throw new ArgumentNullException(nameof(internalPollsApi));
        }

        public async Task<PollResponse> CreatePollAsync(CreatePollRequest createPollRequest)
        {
            var dto = await _internalPollsApi.CreatePollAsync(createPollRequest.TrySaveToDto());
            return dto.ToDomain<PollResponseInternalDTO, PollResponse>();
        }

        public async Task<PollResponse> GetPollAsync(string pollId)
        {
            var dto = await _internalPollsApi.GetPollAsync(pollId);
            return dto.ToDomain<PollResponseInternalDTO, PollResponse>();
        }

        public async Task<PollResponse> UpdatePollAsync(string pollId, UpdatePollRequest updatePollRequest)
        {
            var dto = await _internalPollsApi.UpdatePollAsync(pollId, updatePollRequest.TrySaveToDto());
            return dto.ToDomain<PollResponseInternalDTO, PollResponse>();
        }

        public async Task<PollResponse> UpdatePollPartialAsync(string pollId, UpdatePollPartialRequest updatePollPartialRequest)
        {
            var dto = await _internalPollsApi.UpdatePollPartialAsync(pollId, updatePollPartialRequest.TrySaveToDto());
            return dto.ToDomain<PollResponseInternalDTO, PollResponse>();
        }

        public async Task<ApiResponse> DeletePollAsync(string pollId)
        {
            var dto = await _internalPollsApi.DeletePollAsync(pollId);
            return dto.ToDomain<ResponseInternalDTO, ApiResponse>();
        }

        public async Task<QueryPollsResponse> QueryPollsAsync(QueryPollsRequest queryPollsRequest)
        {
            var dto = await _internalPollsApi.QueryPollsAsync(queryPollsRequest.TrySaveToDto());
            return dto.ToDomain<QueryPollsResponseInternalDTO, QueryPollsResponse>();
        }

        public async Task<PollOptionResponse> CreatePollOptionAsync(string pollId, CreatePollOptionRequest createPollOptionRequest)
        {
            var dto = await _internalPollsApi.CreatePollOptionAsync(pollId, createPollOptionRequest.TrySaveToDto());
            return dto.ToDomain<PollOptionResponseInternalDTO, PollOptionResponse>();
        }

        public async Task<PollOptionResponse> GetPollOptionAsync(string pollId, string optionId)
        {
            var dto = await _internalPollsApi.GetPollOptionAsync(pollId, optionId);
            return dto.ToDomain<PollOptionResponseInternalDTO, PollOptionResponse>();
        }

        public async Task<PollOptionResponse> UpdatePollOptionAsync(string pollId, string optionId, UpdatePollOptionRequest updatePollOptionRequest)
        {
            var dto = await _internalPollsApi.UpdatePollOptionAsync(pollId, optionId, updatePollOptionRequest.TrySaveToDto());
            return dto.ToDomain<PollOptionResponseInternalDTO, PollOptionResponse>();
        }

        public async Task<ApiResponse> DeletePollOptionAsync(string pollId, string optionId)
        {
            var dto = await _internalPollsApi.DeletePollOptionAsync(pollId, optionId);
            return dto.ToDomain<ResponseInternalDTO, ApiResponse>();
        }

        public async Task<PollVoteResponse> CastVoteAsync(string messageId, string pollId, CastPollVoteRequest castPollVoteRequest)
        {
            var dto = await _internalPollsApi.CastVoteAsync(messageId, pollId, castPollVoteRequest.TrySaveToDto());
            return dto.ToDomain<PollVoteResponseInternalDTO, PollVoteResponse>();
        }

        public async Task<PollVoteResponse> RemoveVoteAsync(string messageId, string pollId, string voteId)
        {
            var dto = await _internalPollsApi.RemoveVoteAsync(messageId, pollId, voteId);
            return dto.ToDomain<PollVoteResponseInternalDTO, PollVoteResponse>();
        }

        public async Task<PollVotesResponse> QueryVotesAsync(string pollId, QueryPollVotesRequest queryPollVotesRequest)
        {
            var dto = await _internalPollsApi.QueryVotesAsync(pollId, queryPollVotesRequest.TrySaveToDto());
            return dto.ToDomain<PollVotesResponseInternalDTO, PollVotesResponse>();
        }

        private readonly IInternalPollsApi _internalPollsApi;
    }
}

