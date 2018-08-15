using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DbMappers
{

    /// <summary>
    /// 实体类IEntity属性中值类型DataObject映射
    /// </summary>
    public interface IDataObjectMapper
    {
        /// <summary>
        /// 获取属性信息
        /// </summary>
        /// <returns></returns>
        System.Reflection.PropertyInfo PropertyInfo { get; }
    }
}
