using GetStreamIO.Core.DTO.Requests;

namespace Plugins.GetStreamIO.Core.Requests.V2
{
    public partial class SortParamRequest : RequestObjectBase, ISavableTo<SortParamRequestDTO>
    {
        public int? Direction { get; set; }

        public string Field { get; set; }

        public SortParamRequestDTO SaveToDto() =>
            new SortParamRequestDTO
            {
                Direction = Direction,
                Field = Field,
                AdditionalProperties = AdditionalProperties
            };
    }
}