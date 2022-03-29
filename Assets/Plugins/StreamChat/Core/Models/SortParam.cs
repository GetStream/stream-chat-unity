using StreamChat.Core.DTO.Models;
using StreamChat.Core.Requests;

namespace StreamChat.Core.Models
{
    public class SortParam : RequestObjectBase, ISavableTo<SortParamDTO>
    {
        public int? Direction { get; set; }

        public string Field { get; set; }

        SortParamDTO ISavableTo<SortParamDTO>.SaveToDto() =>
            new SortParamDTO
            {
                Direction = Direction,
                Field = Field,
                AdditionalProperties = AdditionalProperties
            };
    }
}