using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class SortParamRequest : RequestObjectBase, ISavableTo<SortParamRequestInternalDTO>, ISavableTo<SortParamInternalDTO>
    {
        public int? Direction { get; set; }

        public string Field { get; set; }

        SortParamRequestInternalDTO ISavableTo<SortParamRequestInternalDTO>.SaveToDto() =>
            new SortParamRequestInternalDTO
            {
                Direction = Direction,
                Field = Field,
                AdditionalProperties = AdditionalProperties
            };

        SortParamInternalDTO ISavableTo<SortParamInternalDTO>.SaveToDto() =>
            new SortParamInternalDTO
            {
                Direction = Direction,
                Field = Field,
                AdditionalProperties = AdditionalProperties
            };
    }
}