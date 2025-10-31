using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.Requests;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core
{
    /// <summary>
    /// Implementation of Polls API for stateful client
    /// </summary>
    internal class StreamPollsApi : IStreamPollsApi
    {
        public async Task<IStreamPoll> CreatePollAsync(StreamCreatePollRequest createRequest)
        {
            StreamAsserts.AssertNotNull(createRequest, nameof(createRequest));

            var requestDto = createRequest.TrySaveToDto();
            var response = await _lowLevelClient.InternalPollsApi.CreatePollAsync(requestDto);

            return _cache.TryCreateOrUpdate(response.Poll);
        }

        public async Task<IStreamPoll> GetPollAsync(string pollId)
        {
            StreamAsserts.AssertNotNullOrEmpty(pollId, nameof(pollId));

            var response = await _lowLevelClient.InternalPollsApi.GetPollAsync(pollId);

            return _cache.TryCreateOrUpdate(response.Poll);
        }

        public async Task<IEnumerable<IStreamPoll>> QueryPollsAsync(StreamQueryPollsRequest queryRequest)
        {
            StreamAsserts.AssertNotNull(queryRequest, nameof(queryRequest));

            var requestDto = queryRequest.TrySaveToDto();
            var response = await _lowLevelClient.InternalPollsApi.QueryPollsAsync(requestDto);

            if (response.Polls == null || response.Polls.Count == 0)
            {
                return Enumerable.Empty<IStreamPoll>();
            }

            var result = new List<IStreamPoll>();
            foreach (var pollDto in response.Polls)
            {
                result.Add(_cache.TryCreateOrUpdate(pollDto));
            }

            return result;
        }

        internal StreamPollsApi(StreamChatLowLevelClient lowLevelClient, ICache cache)
        {
            _lowLevelClient = lowLevelClient ?? throw new ArgumentNullException(nameof(lowLevelClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        private readonly StreamChatLowLevelClient _lowLevelClient;
        private readonly ICache _cache;
    }
}

