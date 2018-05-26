using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.Conditions
{
    public class SortBy : ISortBy
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
}
