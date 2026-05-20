using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Requests
{
    /// <summary>
    /// Request for <see cref="IStreamChatClient.SearchMessagesAsync"/>.
    ///
    /// <para>
    /// <see cref="ChannelFilter"/> is required by the server - at least one channel-level rule
    /// must be provided (e.g. <c>ChannelFilter.Members.In(localUser)</c>) to scope the search.
    /// </para>
    /// </summary>
    public sealed class StreamSearchMessagesRequest : ISavableTo<SearchRequestInternalDTO>
    {
        /// <summary>
        /// REQUIRED. Filter restricting which channels are searched. Use
        /// <see cref="StreamChat.Core.QueryBuilders.Filters.Channels.ChannelFilter"/> to build the rules.
        ///
        /// Typical: <c>ChannelFilter.Members.In(Client.LocalUserData.User)</c>.
        /// </summary>
        public IEnumerable<IFieldFilterRule> ChannelFilter { get; set; }

        /// <summary>
        /// Optional. Filter restricting which messages within the matched channels match. Use
        /// <see cref="StreamChat.Core.QueryBuilders.Filters.Messages.MessageFilter"/> to build the rules.
        ///
        /// Mutually exclusive at the <c>text</c> field with <see cref="Query"/>; that combination
        /// is rejected client-side before the request is sent.
        /// </summary>
        public IEnumerable<IFieldFilterRule> MessageFilter { get; set; }

        /// <summary>
        /// Optional. Free-text search phrase. Performs full-text search on the message text.
        ///
        /// Cannot be combined with a <see cref="MessageFilter"/> rule targeting the <c>text</c> field.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Optional. Max number of results per page. The server default and recommended max for
        /// offset-based pagination is 30.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Optional. Offset-based pagination. Capped at 1000 total results by the server.
        ///
        /// Mutually exclusive with <see cref="Next"/> and with <see cref="Sort"/> when greater than zero.
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Optional. Cursor for the next page - pass the <see cref="Responses.StreamSearchMessagesResponse.Next"/>
        /// value from a previous response.
        ///
        /// Mutually exclusive with <see cref="Offset"/>.
        /// </summary>
        public string Next { get; set; }

        /// <summary>
        /// Optional. Sort criteria. The server forbids combining a sort with a non-zero
        /// <see cref="Offset"/>; use <see cref="Next"/> for sorted pagination.
        /// </summary>
        public MessagesSortObject Sort { get; set; }

        /// <summary>
        /// Whether the SDK should start watching the channels that appear in the result set so
        /// that the returned <see cref="IStreamMessage"/> instances and their parent
        /// <see cref="IStreamChannel"/> receive realtime WebSocket updates.
        ///
        /// Default: <c>false</c>. Recommended for a search-results UI - watch the channel only when
        /// the user opens one of the hits to avoid mass-watching channels behind the customer's back.
        /// When <c>false</c>, hit messages are still cached as <see cref="IStreamMessage"/>, but their
        /// parent <see cref="IStreamChannel"/> only receives realtime events once explicitly watched
        /// (e.g. via <see cref="IStreamChatClient.GetOrCreateChannelWithIdAsync"/> or
        /// <see cref="IStreamChatClient.QueryChannelsAsync"/>).
        /// </summary>
        public bool WatchResultChannels { get; set; }

        SearchRequestInternalDTO ISavableTo<SearchRequestInternalDTO>.SaveToDto()
        {
            return new SearchRequestInternalDTO
            {
                FilterConditions = ChannelFilter?
                    .Select(_ => _.GenerateFilterEntry())
                    .ToDictionary(x => x.Key, x => x.Value),
                MessageFilterConditions = MessageFilter?
                    .Select(_ => _.GenerateFilterEntry())
                    .ToDictionary(x => x.Key, x => x.Value),
                Query = Query,
                Limit = Limit,
                Offset = Offset,
                Next = Next,
                Sort = Sort?.ToSortParamRequestList(),
            };
        }
    }
}
