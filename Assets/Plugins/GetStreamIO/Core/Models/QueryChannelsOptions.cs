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
    /// Fluent builder extensions for <see cref="QueryChannelsOptions"/>
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