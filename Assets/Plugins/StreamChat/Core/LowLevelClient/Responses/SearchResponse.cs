using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class SearchResponse : ResponseObjectBase, ILoadableFrom<SearchResponseInternalDTO, SearchResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Value to pass to the next search query in order to paginate
        /// </summary>
        public string Next { get; set; }

        /// <summary>
        /// Value that points to the previous page. Pass as the next value in a search query to paginate backwards
        /// </summary>
        public string Previous { get; set; }

        /// <summary>
        /// Search results
        /// </summary>
        public System.Collections.Generic.List<SearchResult> Results { get; set; }

        /// <summary>
        /// Warning about the search results
        /// </summary>
        public SearchWarning ResultsWarning { get; set; }

        SearchResponse ILoadableFrom<SearchResponseInternalDTO, SearchResponse>.LoadFromDto(SearchResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            Next = dto.Next;
            Previous = dto.Previous;
            Results = Results.TryLoadFromDtoCollection(dto.Results);
            ResultsWarning = ResultsWarning.TryLoadFromDto(dto.ResultsWarning);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}