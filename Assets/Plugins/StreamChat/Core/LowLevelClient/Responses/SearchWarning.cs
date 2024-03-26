using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class SearchWarning : ResponseObjectBase, ILoadableFrom<SearchWarningInternalDTO, SearchWarning>
    {
        /// <summary>
        /// Channel CIDs for the searched channels
        /// </summary>
        public System.Collections.Generic.List<string> ChannelSearchCids { get; set; }

        /// <summary>
        /// Number of channels searched
        /// </summary>
        public int? ChannelSearchCount { get; set; }

        /// <summary>
        /// Code corresponding to the warning
        /// </summary>
        public int? WarningCode { get; set; }

        /// <summary>
        /// Description of the warning
        /// </summary>
        public string WarningDescription { get; set; }

        SearchWarning ILoadableFrom<SearchWarningInternalDTO, SearchWarning>.LoadFromDto(SearchWarningInternalDTO dto)
        {
            ChannelSearchCids = dto.ChannelSearchCids;
            ChannelSearchCount = dto.ChannelSearchCount;
            WarningCode = dto.WarningCode;
            WarningDescription = dto.WarningDescription;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}