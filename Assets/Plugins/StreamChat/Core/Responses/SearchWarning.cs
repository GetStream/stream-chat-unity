using StreamChat.Core.DTO.Models;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Responses
{
    public partial class SearchWarning : ResponseObjectBase, ILoadableFrom<SearchWarningDTO, SearchWarning>
    {
        /// <summary>
        /// Channel CIDs for the searched channels
        /// </summary>
        public System.Collections.Generic.List<string> ChannelSearchCids { get; set; }

        /// <summary>
        /// Number of channels searched
        /// </summary>
        public double? ChannelSearchCount { get; set; }

        /// <summary>
        /// Code corresponding to the warning
        /// </summary>
        public double? WarningCode { get; set; }

        /// <summary>
        /// Description of the warning
        /// </summary>
        public string WarningDescription { get; set; }

        SearchWarning ILoadableFrom<SearchWarningDTO, SearchWarning>.LoadFromDto(SearchWarningDTO dto)
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