using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Models
{
    /// <summary>
    /// Allows to sort field <see cref="SortFieldId"/> in <see cref="SortDirection"/>
    /// </summary>
    public class SortOption
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("direction")]
        public SortDirection Direction { get; set; }

        public static SortOption Create(SortFieldId field, SortDirection direction) =>
            new SortOption
            {
                Field = ApiMapper.SortFields[field],
                Direction = direction
            };
    }
}