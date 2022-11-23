using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Requests
{
    public sealed class StreamSortParam : ISavableTo<SortParamInternalDTO>
    {
        public int? Direction { get; set; }

        public string Field { get; set; }

        SortParamInternalDTO ISavableTo<SortParamInternalDTO>.SaveToDto() =>
            new SortParamInternalDTO
            {
                Direction = Direction,
                Field = Field,
            };
    }
}