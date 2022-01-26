using StreamChat.Core;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Responses
{
    public partial class DeleteChannelsResult : ResponseObjectBase, ILoadableFrom<DeleteChannelsResultDTO, DeleteChannelsResult>
    {
        public string Error { get; set; }

        public string Status { get; set; }

        public DeleteChannelsResult LoadFromDto(DeleteChannelsResultDTO dto)
        {
            Error = dto.Error;
            Status = dto.Status;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}