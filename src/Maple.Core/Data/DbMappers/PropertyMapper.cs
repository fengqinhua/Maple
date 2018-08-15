using System;
using System.Data;
using System.Reflection;
using Maple.Core.Reflection;
using Maple.Core.Timing;
namespace Maple.Core.Data.DbMappers
{
    public class PropertyMapper : IPropertyMapper
    {
        private readonly GetValueDelegate getter;
        private readonly SetValueDelegate setter;

        /// <summary>
        /// 实体类的属性
        /// </summary>
        /// <param name="propertyInfo"></param>
        public PropertyMapper(PropertyInfo propertyInfo)
        {
            this.Code = propertyInfo.Name;
            this.ColumnName = propertyInfo.Name;
            this.IsPrimaryKey = propertyInfo.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase);
            this.AllowsNulls = propertyInfo.IsIncludingNullable();
            this.DbType = PropertyTypeToDbTypeTranslator.Translation(propertyInfo.PropertyType);
            //设置缺省长度
            if (this.DbType == System.Data.DbType.String)
                this.Size = 200;
            else
                this.Size = 0;
            this.IsDataObjectProperty = false;

            this.getter = DynamicMethodFactory.CreatePropertyGetter(propertyInfo);
            this.setter = DynamicMethodFactory.CreatePropertySetter(propertyInfo);
        }

        /// <summary>
        /// 实体类对应ValueObject值对象的属性
        /// </summary>
        /// <param name="valueObjectInfo"></param>
        /// <param name="propertyInfo"></param>
        public PropertyMapper(PropertyInfo valueObjectInfo, PropertyInfo propertyInfo)
        {
            this.Code = valueObjectInfo.Name + "_" + propertyInfo.Name;
            this.ColumnName = this.Code;
            this.IsPrimaryKey = false;
            this.AllowsNulls = propertyInfo.IsIncludingNullable();
            this.DbType = PropertyTypeToDbTypeTranslator.Translation(propertyInfo.PropertyType);
            //设置缺省长度
            if (this.DbType == System.Data.DbType.String)
                this.Size = 200;
            else
                this.Size = 0;
            this.IsDataObjectProperty = true;

            this.getter = DynamicMethodFactory.CreatePropertyGetter(valueObjectInfo, propertyInfo);
            this.setter = (target,  arg) => { throw new Exception("valueObject enable edit"); };
        }

        /// <summary>
        /// IPropertyMapper标识
        /// </summary>
        public string Code { get; private set; }
        /// <summary>
        /// 数据库字段名称
        /// </summary>
        public string ColumnName { get;  set; }
        /// <summary>
        /// 是否主键标识
        /// </summary>
        public bool IsPrimaryKey { get; private set; }
        /// <summary>
        /// 是否可空
        /// </summary>
        public bool AllowsNulls { get;  set; }
        /// <summary>
        /// 数据库字段类型
        /// </summary>
        public DbType DbType { get;  set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int Size { get;  set; }
        /// <summary>
        /// 是否为值对象中的属性
        /// </summary>
        public bool IsDataObjectProperty { get; private set; }
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
    }
}
