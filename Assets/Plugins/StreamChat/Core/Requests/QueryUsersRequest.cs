using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Requests
{
    public partial class QueryUsersRequest : RequestObjectBase, ISavableTo<QueryUsersRequestDTO>
    {
        /// <summary>
        /// User filters
        /// </summary>
        public System.Collections.Generic.Dictionary<string, object> FilterConditions { get; set; } = new System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Matches IDs that are greater than the specified ID
        /// </summary>
        public string IdGt { get; set; }

        /// <summary>
        /// Matches IDs that are greater than or equal to the specified ID
        /// </summary>
        public string IdGte { get; set; }

        /// <summary>
        /// Matches IDs that are less than the specified ID
        /// </summary>
        public string IdLt { get; set; }

        /// <summary>
        /// Matches IDs that are less than or equal to the specified ID
        /// </summary>
        public string IdLte { get; set; }

        /// <summary>
        /// Number of records to return
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Number of records to offset
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Request user presence status
        /// </summary>
        public bool? Presence { get; set; }

        /// <summary>
        /// Array of sort parameters
        /// </summary>
        public System.Collections.Generic.List<SortParam> Sort { get; set; } = new System.Collections.Generic.List<SortParam>();

        QueryUsersRequestDTO ISavableTo<QueryUsersRequestDTO>.SaveToDto()
        {
            return new QueryUsersRequestDTO
            {
                FilterConditions = FilterConditions,
                IdGt = IdGt,
                IdGte = IdGte,
                IdLt = IdLt,
                IdLte = IdLte,
                Limit = Limit,
                Offset = Offset,
                Presence = Presence,
                Sort = Sort.TrySaveToDtoCollection<SortParam, SortParamDTO>(),
                AdditionalProperties = AdditionalProperties,
            };
        }
    }
}