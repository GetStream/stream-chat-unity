using GetStreamIO.Core.DTO.Requests;

namespace Plugins.GetStreamIO.Core.Requests
{
    public partial class PaginationParamsRequest : RequestObjectBase, ISavableTo<PaginationParamsRequestDTO>
    {
        public double? IdGt { get; set; }

        public double? IdGte { get; set; }

        public double? IdLt { get; set; }

        public double? IdLte { get; set; }

        public double? Limit { get; set; }

        public double? Offset { get; set; }

        public PaginationParamsRequestDTO SaveToDto() =>
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