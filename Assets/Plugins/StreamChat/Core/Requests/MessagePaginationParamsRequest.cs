using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class MessagePaginationParamsRequest : RequestObjectBase, ISavableTo<MessagePaginationParamsRequestDTO>
    {
        public System.DateTimeOffset? CreatedAtAfter { get; set; }

        public System.DateTimeOffset? CreatedAtAfterOrEqual { get; set; }

        public System.DateTimeOffset? CreatedAtAround { get; set; }

        public System.DateTimeOffset? CreatedAtBefore { get; set; }

        public System.DateTimeOffset? CreatedAtBeforeOrEqual { get; set; }

        public string IdAround { get; set; }

        public string IdGt { get; set; }

        public string IdGte { get; set; }

        public string IdLt { get; set; }

        public string IdLte { get; set; }

        public int? Limit { get; set; }

        public int? Offset { get; set; }

        MessagePaginationParamsRequestDTO ISavableTo<MessagePaginationParamsRequestDTO>.SaveToDto() =>
            new MessagePaginationParamsRequestDTO
            {
                AdditionalProperties = AdditionalProperties,
                CreatedAtAfter = CreatedAtAfter,
                CreatedAtAfterOrEqual = CreatedAtAfterOrEqual,
                CreatedAtAround = CreatedAtAround,
                CreatedAtBefore = CreatedAtBefore,
                CreatedAtBeforeOrEqual = CreatedAtBeforeOrEqual,
                IdAround = IdAround,
                IdGt = IdGt,
                IdGte = IdGte,
                IdLt = IdLt,
                IdLte = IdLte,
                Limit = Limit,
                Offset = Offset,
            };
    }
}