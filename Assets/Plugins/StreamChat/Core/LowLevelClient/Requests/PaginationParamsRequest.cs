using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class PaginationParamsRequest : RequestObjectBase, ISavableTo<PaginationParamsRequestInternalDTO>
    {
        public int? IdGt { get; set; }

        public int? IdGte { get; set; }

        public int? IdLt { get; set; }

        public int? IdLte { get; set; }

        public int? Limit { get; set; }

        public int? Offset { get; set; }

        PaginationParamsRequestInternalDTO ISavableTo<PaginationParamsRequestInternalDTO>.SaveToDto() =>
            new PaginationParamsRequestInternalDTO
            {
                IdGt = IdGt,
                IdGte = IdGte,
                IdLt = IdLt,
                IdLte = IdLte,
                Limit = Limit,
                Offset = Offset,
                AdditionalProperties = AdditionalProperties
            };
    }
}