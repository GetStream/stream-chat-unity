using StreamChat.Core;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Requests;

namespace Plugins.StreamChat.Core.Models
{
    public partial class SortParam : RequestObjectBase, ISavableTo<SortParamDTO>
    {
        public int? Direction { get; set; }

        public string Field { get; set; }

        public SortParamDTO SaveToDto() =>
            new SortParamDTO
            {
                Direction = Direction,
                Field = Field,
                AdditionalProperties = AdditionalProperties
            };
    }
}