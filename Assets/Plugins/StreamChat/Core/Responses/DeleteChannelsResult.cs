using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.Responses
{
    public partial class DeleteChannelsResult : ResponseObjectBase, ILoadableFrom<DeleteChannelsResultDTO, DeleteChannelsResult>
    {
        public string Error { get; set; }

        public string Status { get; set; }

        DeleteChannelsResult ILoadableFrom<DeleteChannelsResultDTO, DeleteChannelsResult>.LoadFromDto(DeleteChannelsResultDTO dto)
        {
            Error = dto.Error;
            Status = dto.Status;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}