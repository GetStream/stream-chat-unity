using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Requests
{
    public sealed class StreamSortParam : ISavableTo<SortParamRequestInternalDTO>
    {
        public int? Direction { get; set; }

        public string Field { get; set; }

        SortParamRequestInternalDTO ISavableTo<SortParamRequestInternalDTO>.SaveToDto() =>
            new SortParamRequestInternalDTO
            {
                Direction = Direction,
                Field = Field,
            };
    }
}