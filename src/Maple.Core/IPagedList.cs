using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core
{
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagedList<T> : IList<T>
    {
        /// <summary>
        /// 第几页
        /// </summary>
        int PageIndex { get; }
        /// <summary>
        /// 页码大小
        /// </summary>
        int PageSize { get; }
        /// <summary>
        /// 记录总数
        /// </summary>
        int TotalCount { get; }
        /// <summary>
        /// 页码总数
        /// </summary>
        int TotalPages { get; }
        /// <summary>
        /// 是否有上一页
        /// </summary>
        bool HasPreviousPage { get; }
        /// <summary>
        /// 是否有下一页
        /// </summary>
        bool HasNextPage { get; }
    }
}
