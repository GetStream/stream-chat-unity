using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Requests;

namespace StreamChat.Core.LowLevelClient.Models
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