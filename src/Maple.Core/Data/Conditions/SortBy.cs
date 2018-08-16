using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.Conditions
{
    public class SortBy 
    {
        public SortBy(string key) : this(key, FieldSearchOrder.Ascending) { }

        public SortBy(string key, FieldSearchOrder searchOrder)
        {
            this.Key = key;
            this.SearchOrder = searchOrder;
        }

        /// <summary>
        /// 排序主键
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        public FieldSearchOrder SearchOrder { get; private set; }
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
