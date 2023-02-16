using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Base class for query sort objects
    /// </summary>
    public abstract class QuerySort<TSortType, TFieldType> where TSortType : QuerySort<TSortType, TFieldType>
    {
        /// <summary>
        /// Order by field in an ascending order
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        internal TSortType OrderByAscending(TFieldType fieldName)
        {
            Instance._order.Add((fieldName, AscendingOrder));
            return Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        internal TSortType OrderByDescending(TFieldType fieldName)
        {
            Instance._order.Add((fieldName, DescendingOrder));
            return Instance;
        }

        internal List<SortParamRequestInternalDTO> ToSortParamRequestList()
        {
            if (_order.Count == 0)
            {
                return null;
            }

            var sortParams = new List<SortParamRequestInternalDTO>();

            foreach (var entry in _order)
            {
                sortParams.Add(new SortParamRequestInternalDTO
                {
                    Direction = entry.Direction,
                    Field = ToUnderlyingFieldName(entry.Field),
                });
            }

            return sortParams;
        }
        
        internal List<SortParamInternalDTO> ToSortParamInternalDTOs()
        {
            if (_order.Count == 0)
            {
                return null;
            }

            var sortParams = new List<SortParamInternalDTO>();

            foreach (var entry in _order)
            {
                sortParams.Add(new SortParamInternalDTO
                {
                    Direction = entry.Direction,
                    Field = ToUnderlyingFieldName(entry.Field),
                });
            }

            return sortParams;
        }

        protected abstract TSortType Instance { get; }

        protected abstract string ToUnderlyingFieldName(TFieldType field);

        private const int AscendingOrder = 1;
        private const int DescendingOrder = -1;

        private readonly List<(TFieldType Field, int Direction)> _order = new List<(TFieldType Field, int Direction)>();
    }
}