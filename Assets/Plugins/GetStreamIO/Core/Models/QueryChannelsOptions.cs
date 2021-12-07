using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Models
{
    /// <summary>
    /// Options passed with get channels query
    /// </summary>
    public class QueryChannelsOptions
    {
        public const int DefaultOffset = 0;
        public const int DefaultLimit = 30;

        //Todo: We could implement filter builder with fluent pattern
        //https://getstream.io/chat/docs/other-rest/query_channels/?language=kotlin#channel-queryable-built-in-fields

        [JsonProperty("filter_conditions")]
        public IDictionary<string, object> Filter { get; } = new Dictionary<string, object>();

        [JsonProperty("limit")]
        public int Limit = DefaultLimit;

        [JsonProperty("offset")]
        public int Offset = DefaultOffset;

        [JsonProperty("state")]
        public bool State = true;

        [JsonProperty("watch")]
        public bool Watch = true;

        [JsonProperty("sort")]
        public IList<SortOption> Sort { get; } = new List<SortOption>();

        public static QueryChannelsOptions Default => new QueryChannelsOptions();
    }

    /// <summary>
    /// Fluent builder for <see cref="QueryChannelsOptions"/>
    /// </summary>
    public static class QueryChannelsOptionsBuilder
    {
        public static QueryChannelsOptions SortBy(this QueryChannelsOptions options, SortFieldId fieldId, SortDirection direction)
        {
            options.Sort.Add(SortOption.Create(fieldId, direction));
            return options;
        }
    }
}