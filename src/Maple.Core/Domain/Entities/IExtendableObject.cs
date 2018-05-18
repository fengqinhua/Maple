using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Entities
{
    /// <summary>
    /// 定义一个JSON格式的字符串属性来扩展对象/实体
    /// </summary>
    public interface IExtendableObject
    {
        /// <summary>
        /// 一个JSON格式化的字符串来扩展对象/实体
        /// JSON字符串以Key/Value的形式存在，格式如下：
        /// <code>
        /// {
        ///   "Property1" : ...
        ///   "Property2" : ...
        /// }
        /// </code>
        /// </summary>
        string ExtensionData { get; set; }
    }
}
