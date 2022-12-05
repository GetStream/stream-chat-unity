using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class QueryUsersRequest : RequestObjectBase, ISavableTo<QueryUsersRequestInternalDTO>
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
        public System.Collections.Generic.List<SortParamRequest> Sort { get; set; } = new System.Collections.Generic.List<SortParamRequest>();

        QueryUsersRequestInternalDTO ISavableTo<QueryUsersRequestInternalDTO>.SaveToDto()
        {
            return new QueryUsersRequestInternalDTO
            {
                FilterConditions = FilterConditions,
                IdGt = IdGt,
                IdGte = IdGte,
                IdLt = IdLt,
                IdLte = IdLte,
                Limit = Limit,
                Offset = Offset,
                Presence = Presence,
                Sort = Sort.TrySaveToDtoCollection<SortParamRequest, SortParamInternalDTO>(),
                AdditionalProperties = AdditionalProperties,
            };
        }
    }
}