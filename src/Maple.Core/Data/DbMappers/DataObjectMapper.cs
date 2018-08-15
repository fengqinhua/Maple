using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Maple.Core.Data.DbMappers
{
    public class DataObjectMapper: IDataObjectMapper
    {
        public DataObjectMapper(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// 获取熟悉信息
        /// </summary>
        /// <returns></returns>
        public PropertyInfo PropertyInfo { get; private set; }
    }
}
