using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core
{
    /// <summary>
    /// Implementation of Threads API for stateful client
    /// </summary>
    internal class StreamThreadsApi : IStreamThreadsApi
    {
        public async Task<IStreamThread> GetThreadAsync(string parentMessageId,
            int? replyLimit = null,
            int? participantLimit = null,
            int? memberLimit = null,
            bool watch = true)
        {
            StreamAsserts.AssertNotNullOrEmpty(parentMessageId, nameof(parentMessageId));

            var response = await _lowLevelClient.InternalThreadsApi.GetThreadAsync(parentMessageId,
                replyLimit: replyLimit,
                participantLimit: participantLimit,
                memberLimit: memberLimit,
                watch: watch);

            return _cache.TryCreateOrUpdate(response.Thread);
        }

        public async Task<StreamQueryThreadsResponse> QueryThreadsAsync(StreamQueryThreadsRequest request)
        {
            StreamAsserts.AssertNotNull(request, nameof(request));

            var requestDto = request.TrySaveToDto();
            var response = await _lowLevelClient.InternalThreadsApi.QueryThreadsAsync(requestDto);

            var threads = new List<IStreamThread>();
            if (response.Threads != null)
            {
                foreach (var threadDto in response.Threads)
                {
                    var thread = _cache.TryCreateOrUpdate(threadDto);
                    if (thread != null)
                    {
                        threads.Add(thread);
                    }
                }
            }

            return new StreamQueryThreadsResponse
            {
                Threads = threads,
                Next = response.Next,
                Prev = response.Prev,
            };
        }

        internal StreamThreadsApi(StreamChatLowLevelClient lowLevelClient, ICache cache)
        {
            _lowLevelClient = lowLevelClient ?? throw new ArgumentNullException(nameof(lowLevelClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        private readonly StreamChatLowLevelClient _lowLevelClient;
        private readonly ICache _cache;
    }
}
