using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class SortParamRequest : RequestObjectBase, ISavableTo<SortParamRequestInternalDTO>
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
    }
}