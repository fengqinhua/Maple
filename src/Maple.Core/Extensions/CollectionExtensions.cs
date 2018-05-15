using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core
{
    /// <summary>
    /// 集合 Collection的扩展方法
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 检查集合对象是否为空或者无元素
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        /// 判断元素是否存在，如果不存在则添加
        /// </summary>
        /// <param name="source">Collection</param>
        /// <param name="item">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added, returns False if not.</returns>
        public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source.Contains(item))
            {
                return false;
            }

            source.Add(item);
            return true;
        }
    }
}
