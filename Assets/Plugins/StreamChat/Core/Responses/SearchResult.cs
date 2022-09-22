using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Responses
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