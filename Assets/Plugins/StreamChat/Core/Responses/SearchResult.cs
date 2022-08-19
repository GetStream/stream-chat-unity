using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Responses
{
    public partial class SearchResult : ResponseObjectBase, ILoadableFrom<SearchResultDTO, SearchResult>
    {
        /// <summary>
        /// Found message
        /// </summary>
        public Message Message { get; set; }

        SearchResult ILoadableFrom<SearchResultDTO, SearchResult>.LoadFromDto(SearchResultDTO dto)
        {
            Message = Message.TryLoadFromDto<SearchResultMessageDTO, Message>(dto.Message);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}