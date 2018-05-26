using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.Conditions
{
    public interface ISortBy
    {
        string Key { get; }
        FieldSearchOrder SearchOrder { get; }
    }

    /// <summary>
    /// 列排序方法
    /// </summary>
    public enum FieldSearchOrder
    {
        /// <summary>
        /// 顺序
        /// </summary>
        Ascending = 1,
        /// <summary>
        /// 倒序
        /// </summary>
        Descending = 2
    }
}
