using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class SortParamRequest : RequestObjectBase, ISavableTo<SortParamRequestDTO>, ISavableTo<SortParamDTO>
    {
        public int? Direction { get; set; }

        public string Field { get; set; }

        SortParamRequestDTO ISavableTo<SortParamRequestDTO>.SaveToDto() =>
            new SortParamRequestDTO
            {
                Direction = Direction,
                Field = Field,
                AdditionalProperties = AdditionalProperties
            };

        SortParamDTO ISavableTo<SortParamDTO>.SaveToDto() =>
            new SortParamDTO
            {
                Direction = Direction,
                Field = Field,
                AdditionalProperties = AdditionalProperties
            };
    }
}