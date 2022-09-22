using StreamChat.Core.DTO.Models;
using StreamChat.Core.Requests;

namespace StreamChat.Core.Models
{
    public class SortParam : RequestObjectBase, ISavableTo<SortParamInternalDTO>
    {
        public int? Direction { get; set; }

        public string Field { get; set; }

        SortParamInternalDTO ISavableTo<SortParamInternalDTO>.SaveToDto() =>
            new SortParamInternalDTO
            {
                Direction = Direction,
                Field = Field,
                AdditionalProperties = AdditionalProperties
            };
    }
}