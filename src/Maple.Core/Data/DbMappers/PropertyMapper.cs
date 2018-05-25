using System;
using System.Reflection;
using Maple.Core.Reflection;
using Maple.Core.Timing;
namespace Maple.Core.Data.DbMappers
{
    public class PropertyMapper : IPropertyMapper
    {
        private readonly GetValueDelegate getter;
        private readonly SetValueDelegate setter;

        public PropertyMapper(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.getter = DynamicMethodFactory.CreatePropertyGetter(propertyInfo);
            this.setter = DynamicMethodFactory.CreatePropertySetter(propertyInfo);
            this.ColumnName = propertyInfo.Name;
            this.IsPrimaryKey = propertyInfo.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase);
            this.AllowsNulls = propertyInfo.IsIncludingNullable();
            this.DbType = PropertyTypeToDbTypeTranslator.Translation(propertyInfo.PropertyType);
            //设置缺省长度
            if (this.DbType == System.Data.DbType.String)
                this.Size = 200;
            else
                this.Size = 0;
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
        /// 数据库字段类型
        /// </summary>
        public System.Data.DbType DbType { get; private set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// 读取属性值
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public object FastGetValue(object entity)
        {
            return this.getter(entity);
        }
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="value"></param>
        public void FastSetValue(object entity, object value)
        {
            if (DBNull.Value != value)
            {
                this.setter(entity, value);
            }
        }

        /// <summary>
        /// 设置数据库字段名称
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public PropertyMapper DbColumn(string columnName)
        {
            ColumnName = columnName;
            return this;
        }

        /// <summary>
        /// 设置数据库字段长度
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public PropertyMapper DbSize(int size)
        {
            Size = size;
            return this;
        }
    }
}
