using System;
using System.Reflection;
using Maple.Core.Timing;
namespace Maple.Core.Data.DbMappers
{
    public class PropertyMapper: IPropertyMapper
    {
        public PropertyMapper(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.ColumnName = propertyInfo.Name;
            this.IsPrimaryKey = propertyInfo.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase);
            this.AllowsNulls = propertyInfo.IsIncludingNullable();
        }



        //private void setDefalutValue(Type type)
        //{
        //    if (type.IsEnum)
        //        this.Default = 0;
        //    if (type == typeof(string))
        //        this.Default = "";
        //    if (type == typeof(bool))
        //        this.Default = false;
        //    if (type == typeof(byte))
        //        this.Default = (byte)0;
        //    if (type == typeof(int))
        //        this.Default = (int)0;
        //    if (type == typeof(double))
        //        this.Default = (double)0;
        //    if (type == typeof(decimal))
        //        this.Default = (decimal)0m;
        //    if (type == typeof(float))
        //        this.Default = (float)0f;
        //    if (type == typeof(Guid))
        //        this.Default = Guid.NewGuid();
        //    if (type == typeof(DateTime))
        //        this.Default = Clock.INVALID_DATETIME;
        //}

        /// <summary>
        /// 获取熟悉信息
        /// </summary>
        /// <returns></returns>
        public PropertyInfo PropertyInfo { get; private set; }
        /// <summary>
        /// 数据库字段名称
        /// </summary>
        public string ColumnName { get; private set; }
        /// <summary>
        /// 是否主键标识
        /// </summary>
        public bool IsPrimaryKey { get; private set; }
        /// <summary>
        /// 是否可空
        /// </summary>
        public bool AllowsNulls { get; private set; }

        /// <summary>
        /// 设置数据库字段名称
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public PropertyMapper Column(string columnName)
        {
            ColumnName = columnName;
            return this;
        }
    }
}
