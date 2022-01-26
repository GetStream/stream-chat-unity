using Plugins.StreamChat.Core.Models;
using StreamChat.Core;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;
using StreamChat.Core.Requests;

namespace Plugins.StreamChat.Core.Requests
{
    public partial class QueryUsersRequest : RequestObjectBase, ISavableTo<QueryUsersRequestDTO>
    {
        /// <summary>
        /// Websocket connection ID to interact with. You can pass it as body or URL parameter
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// User filters
        /// </summary>
        public System.Collections.Generic.IDictionary<string, object> FilterConditions { get; set; } = new System.Collections.Generic.Dictionary<string, object>();

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
        public double? Limit { get; set; }

        /// <summary>
        /// Number of records to offset
        /// </summary>
        public double? Offset { get; set; }

        /// <summary>
        /// Request user presence status
        /// </summary>
        public bool? Presence { get; set; }

        /// <summary>
        /// Array of sort parameters
        /// </summary>
        public System.Collections.Generic.ICollection<SortParam> Sort { get; set; } = new System.Collections.ObjectModel.Collection<SortParam>();

        public QueryUsersRequestDTO SaveToDto()
        {
            return new QueryUsersRequestDTO
            {
                ConnectionId = ConnectionId,
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