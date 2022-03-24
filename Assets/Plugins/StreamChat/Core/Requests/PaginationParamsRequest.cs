using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class PaginationParamsRequest : RequestObjectBase, ISavableTo<PaginationParamsRequestDTO>
    {
        public int? IdGt { get; set; }

        public int? IdGte { get; set; }

        public int? IdLt { get; set; }

        public int? IdLte { get; set; }

        public int? Limit { get; set; }

        public int? Offset { get; set; }

        PaginationParamsRequestDTO ISavableTo<PaginationParamsRequestDTO>.SaveToDto() =>
            new PaginationParamsRequestDTO
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