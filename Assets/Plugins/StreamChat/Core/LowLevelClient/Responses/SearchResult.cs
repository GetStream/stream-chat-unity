using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class SearchResult : ResponseObjectBase, ILoadableFrom<SearchResultInternalDTO, SearchResult>
    {
        /// <summary>
        /// Found message
        /// </summary>
        public Message Message { get; set; }

        SearchResult ILoadableFrom<SearchResultInternalDTO, SearchResult>.LoadFromDto(SearchResultInternalDTO dto)
        {
            Message = Message.TryLoadFromDto<SearchResultMessageInternalDTO, Message>(dto.Message);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}