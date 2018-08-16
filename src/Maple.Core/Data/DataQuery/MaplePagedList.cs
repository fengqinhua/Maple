using Maple.Core.Data.Conditions;
using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataQuery
{
    [Serializable]
    public class MaplePagedList<TEntity, TPrimaryKey> : List<TEntity>, IPagedList<TEntity> where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public MaplePagedList(IMappeAfterOrderBy<TEntity, TPrimaryKey> source, int pageIndex, int pageSize)
        {
            var total = source.Count();
            this.TotalCount = total;
            this.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;

            int startWith = 0;                      //起始行数
            if (pageIndex > 1)
                startWith = pageSize * (pageIndex - 1);
            if (startWith < total)
            {
                int tn = startWith + pageSize;      //结束行数
                if (tn > total)
                    tn = total;

                var list = source.Range(new Range(startWith + 1, tn)).Select();
                this.AddRange(list);
            }
        }

        /// <summary>
        /// Page index
        /// </summary>
        public int PageIndex { get; }
        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; }
        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get; }
        /// <summary>
        /// Total pages
        /// </summary>
        public int TotalPages { get; }
        /// <summary>
        /// Has previous page
        /// </summary>
        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }
        /// <summary>
        /// Has next page
        /// </summary>
        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }
    }
}
