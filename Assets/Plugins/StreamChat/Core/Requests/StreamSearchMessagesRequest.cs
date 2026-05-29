using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Utils;

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
        /// Optional. Structured, strongly-typed filter applied to individual messages inside the
        /// channels selected by <see cref="ChannelFilter"/>. This is the "WHERE" clause for the
        /// message itself: only messages that match every rule in the list are returned.
        ///
        /// <para>
        /// Build rules with <see cref="StreamChat.Core.QueryBuilders.Filters.Messages.MessageFilter"/>.
        /// Common use cases include:
        /// </para>
        /// <list type="bullet">
        /// <item><description>
        /// Mentions of a specific user — <c>MessageFilter.MentionedUserId.Contains(userId)</c>.
        /// </description></item>
        /// <item><description>
        /// Messages by a given author or set of authors —
        /// <c>MessageFilter.UserId.EqualsTo("alice")</c> or
        /// <c>MessageFilter.UserId.In(new[] { "alice", "bob" })</c>.
        /// </description></item>
        /// <item><description>
        /// Messages of a specific type — <c>MessageFilter.Type.EqualsTo("regular")</c> /
        /// <c>"system"</c> / <c>"deleted"</c>.
        /// </description></item>
        /// <item><description>
        /// Replies only or top-level only — <c>MessageFilter.ParentId.Exists(true)</c> for
        /// thread replies, <c>MessageFilter.ParentId.Exists(false)</c> for top-level messages.
        /// </description></item>
        /// <item><description>
        /// Date ranges — <c>MessageFilter.CreatedAt.GreaterThanOrEquals(from)</c> combined with
        /// <c>MessageFilter.CreatedAt.LessThanOrEquals(to)</c>.
        /// </description></item>
        /// <item><description>
        /// Attachments of a given type — <c>MessageFilter.AttachmentType.In(new[] { "image", "video" })</c>.
        /// </description></item>
        /// <item><description>
        /// Pinned, silent, or polls — <c>MessageFilter.Pinned.EqualsTo(true)</c>,
        /// <c>MessageFilter.Silent.EqualsTo(false)</c>, <c>MessageFilter.PollId.Exists(true)</c>.
        /// </description></item>
        /// <item><description>
        /// Reactions of a given type —
        /// <c>MessageFilter.ReactionType.Contains("fire")</c>.
        /// </description></item>
        /// <item><description>
        /// Custom message fields — <c>MessageFilter.Custom("priority").EqualsTo("high")</c>.
        /// </description></item>
        /// <item><description>
        /// Text matching as a structured rule — <c>MessageFilter.Text.Contains("invoice")</c>.
        /// Use this form when you also need other rules in the same request (see remark below).
        /// </description></item>
        /// </list>
        ///
        /// <para>
        /// All rules in the list are combined with logical AND. For OR / NOR combinations, use the
        /// compound builders on the filter façade.
        /// </para>
        ///
        /// <para>
        /// Remark: mutually exclusive with <see cref="Query"/>. The server rejects requests that
        /// specify both a free-text <c>query</c> and <c>message_filter_conditions</c>; the SDK
        /// catches this client-side and throws <see cref="System.ArgumentException"/>. If you need
        /// text matching alongside other constraints, drop <see cref="Query"/> and use
        /// <c>MessageFilter.Text.Contains(...)</c> here instead.
        /// </para>
        /// </summary>
        public IEnumerable<IFieldFilterRule> MessageFilter { get; set; }

        /// <summary>
        /// Optional. Free-text search phrase executed by the server's full-text search engine
        /// against the message body. This is the shortest path to a "search bar" experience —
        /// the user types a phrase and the server returns the most relevant matching messages
        /// across every channel selected by <see cref="ChannelFilter"/>.
        ///
        /// <para>
        /// Use this when:
        /// </para>
        /// <list type="bullet">
        /// <item><description>
        /// You only need to match on message text and want server-side ranking (relevance,
        /// stemming, fuzzy matching where supported) rather than the literal substring matching
        /// of <c>MessageFilter.Text.Contains(...)</c>.
        /// </description></item>
        /// <item><description>
        /// You are wiring up a generic search input — pass whatever the user typed verbatim.
        /// </description></item>
        /// <item><description>
        /// You want to combine free-text search with channel-level constraints only
        /// (e.g. "search 'release notes' in channels I'm a member of") — supply
        /// <see cref="ChannelFilter"/> rules and leave <see cref="MessageFilter"/> null.
        /// </description></item>
        /// </list>
        ///
        /// <para>
        /// Pair with <see cref="Sort"/> set to <c>MessagesSort.OrderByDescending(MessageSortFieldName.Relevance)</c>
        /// to surface the best matches first.
        /// </para>
        ///
        /// <para>
        /// Remark: mutually exclusive with <see cref="MessageFilter"/>. The server rejects
        /// requests that specify both; the SDK catches this client-side and throws
        /// <see cref="System.ArgumentException"/>. If you need to combine text matching with
        /// other message-level constraints, omit <see cref="Query"/> and express the text rule
        /// inside <see cref="MessageFilter"/> via <c>MessageFilter.Text.Contains(...)</c>.
        /// </para>
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
        /// <para>
        /// Default: <c>true</c>. This keeps the SDK's "stateful = reactive" contract intact - every
        /// <see cref="IStreamMessage"/> / <see cref="IStreamChannel"/> returned here behaves the same
        /// as one obtained through <see cref="IStreamChatClient.QueryChannelsAsync"/> or
        /// <see cref="IStreamChatClient.GetOrCreateChannelWithIdAsync"/>: it fires events, stays in
        /// sync with the server, and shows up in <see cref="IStreamChatClient.WatchedChannels"/>.
        /// Mirrors the behaviour of <c>MessageSearchSource</c> in the JavaScript SDK.
        /// </para>
        ///
        /// <para>
        /// Set to <c>false</c> when a search UI shouldn't subscribe to every result channel up front -
        /// e.g. a "search bar" where the user opens one hit at a time. In that mode the result
        /// <see cref="IStreamMessage"/> and its parent <see cref="IStreamChannel"/> will not receive
        /// realtime updates until the channel is explicitly watched. Call
        /// <see cref="IStreamChannel.WatchAsync"/> on the result's <see cref="IStreamChannel"/>
        /// when the user opens a hit to start receiving updates on that same instance. Use
        /// <see cref="IStreamChannel.IsWatched"/> / <see cref="IStreamMessage.IsWatched"/> to check
        /// whether a given instance is currently receiving updates.
        /// </para>
        ///
        /// <para>
        /// Cost when <c>true</c>: one channel watch round-trip per distinct CID in the result set
        /// (parallelised internally).
        /// </para>
        /// </summary>
        public bool WatchResultChannels { get; set; } = true;

        SearchRequestInternalDTO ISavableTo<SearchRequestInternalDTO>.SaveToDto()
        {
            // POST /search rejects the "+00:00" offset form on date values inside
            // message_filter_conditions / filter_conditions with
            // "field \"created_at\" expects type date" (HTTP 400, code 4). It only accepts
            // the canonical "Z" UTC form, so opt into StreamDateFormat.Utc here. This is the
            // opposite of every other endpoint, which crashes (HTTP 500) on the "Z" form.
            return new SearchRequestInternalDTO
            {
                FilterConditions = ChannelFilter?
                    .Select(_ => _.GenerateFilterEntry(StreamDateFormat.Utc))
                    .ToDictionary(x => x.Key, x => x.Value),
                MessageFilterConditions = ToMessageFilterConditionsOrNullIfEmpty(MessageFilter),
                Query = Query,
                Limit = Limit,
                Offset = Offset,
                Next = Next,
                Sort = Sort?.ToSortParamRequestList(),
            };
        }

        private static Dictionary<string, object> ToMessageFilterConditionsOrNullIfEmpty(
            IEnumerable<IFieldFilterRule> rules)
        {
            var conditions = rules?
                .Where(_ => _ != null)
                .Select(_ => _.GenerateFilterEntry(StreamDateFormat.Utc))
                .ToDictionary(x => x.Key, x => x.Value);

            return conditions != null && conditions.Count > 0 ? conditions : null;
        }
    }
}
