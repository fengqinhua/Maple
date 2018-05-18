using System;
using System.Reflection;
using System.Collections.Generic;
using Maple.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maple.Core.Data.DbMappers
{
    public class AutoEntityMapper<TEntity, TPrimaryKey> : IEntityMapper<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 数据库架构名称
        /// </summary>
        public string SchemaName { get; protected set; }
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; protected set; }
        /// <summary>
        /// 标识列对应属性集合（主键）
        /// </summary>
        public IList<IPropertyMapper> PKeyProperties { get; private set; }
        /// <summary>
        /// 除标识列以外的属性集合
        /// </summary>
        public IList<IPropertyMapper> OtherProperties { get; private set; }
        /// <summary>
        /// 实体类的类型
        /// </summary>
        public Type EntityType
        {
            get { return typeof(IEntity<TPrimaryKey>); }
        }

        public AutoEntityMapper()
        {
            PKeyProperties = new List<IPropertyMapper>();
            OtherProperties = new List<IPropertyMapper>();

            Schema(string.Empty);
            Table(this.EntityType.Name);
            AutoMap();
        }

        public virtual void Schema(string schemaName)
        {
            SchemaName = schemaName;
        }

        public virtual void Table(string tableName)
        {
            TableName = tableName;
        }

        protected virtual void AutoMap(Func<Type, PropertyInfo, bool> canMap = null)
        {
            Type type = this.EntityType;
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var propertyInfo in properties)
            {
                //暂时只支持值类型映射
                if (propertyInfo.IsPrimitiveExtendedIncludingNullable())
                    continue;
                //如果属性无法读写则忽略
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                    continue;
                //判断属性是否包含NotDbFieldAttribute标签，如果有则忽略
                if (propertyInfo.HasAttribute<NotMappedAttribute>(true))
                    continue;

                if ((canMap != null && !canMap(type, propertyInfo)))
                    continue;

                IPropertyMapper pMap = new PropertyMapper(propertyInfo);
                if (pMap.IsPrimaryKey)
                    PKeyProperties.Add(pMap);
                else
                    OtherProperties.Add(pMap);
            }
        }
    }
}
