﻿using System;
using System.Reflection;
using System.Collections.Generic;
using Maple.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Maple.Core.Domain.Values;
using Maple.Core.Reflection;

namespace Maple.Core.Data.DbMappers
{
    public class AutoEntityMapper : IEntityMapper
    {
        private DataReaderDeserializer deserializer = null;

        /// <summary>
        /// 数据库架构名称
        /// </summary>
        public string SchemaName { get; set; }
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 标识列对应属性集合（主键）
        /// </summary>
        public IReadOnlyList<IPropertyMapper> PKeyProperties { get; private set; }
        /// <summary>
        /// 除标识列以外的属性集合
        /// </summary>
        public IReadOnlyList<IPropertyMapper> OtherProperties { get; private set; }
        /// <summary>
        /// 值类型DataObject的属性集合
        /// </summary>
        public IReadOnlyList<IDataObjectMapper> DataObjectProperties { get; private set; }
        /// <summary>
        /// 实体类的类型
        /// </summary>
        public Type EntityType { get; private set; }

        public AutoEntityMapper(Type entityType)
        {
            this.EntityType = entityType;
            this.SchemaName = string.Empty;
            this.TableName = this.EntityType.Name;
            AutoMap();
        }

        protected virtual void AutoMapDataObject(PropertyInfo dataObjectPropertyInfo, List<IPropertyMapper> otherProperties)
        {
            var properties = dataObjectPropertyInfo.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var propertyInfo in properties)
            {
                //如果属性无法读写则忽略
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                    continue;
                //判断属性是否包含NotDbFieldAttribute标签，如果有则忽略
                if (propertyInfo.HasAttribute<NotMappedAttribute>(true))
                    continue;

                if (propertyInfo.IsPrimitiveExtendedIncludingNullableOrEnum())
                {
                    //基元对象
                    IPropertyMapper pMap = new PropertyMapper(dataObjectPropertyInfo, propertyInfo);
                    otherProperties.Add(pMap);
                }
            }
        }

        protected virtual void AutoMap(Func<Type, PropertyInfo, bool> canMap = null)
        {
            Type type = this.EntityType;
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            List<IPropertyMapper> pkeyProperties = new List<IPropertyMapper>();
            List<IPropertyMapper> otherProperties = new List<IPropertyMapper>();
            List<IDataObjectMapper> dataObjectProperties = new List<IDataObjectMapper>();

            foreach (var propertyInfo in properties)
            {
                //如果属性无法读写则忽略
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                    continue;
                //判断属性是否包含NotDbFieldAttribute标签，如果有则忽略
                if (propertyInfo.HasAttribute<NotMappedAttribute>(true))
                    continue;

                if ((canMap != null && !canMap(type, propertyInfo)))
                    continue;

                if (propertyInfo.PropertyType.IsTypeDerivedFromGenericType(typeof(ValueObject<>)))
                {
                    //值对象
                    IDataObjectMapper dataObjectMapper = new DataObjectMapper(propertyInfo);
                    dataObjectProperties.Add(dataObjectMapper);
                    //添加值对象DataObject的属性
                    AutoMapDataObject(propertyInfo, otherProperties);
                }
                else
                {
                    if (propertyInfo.IsPrimitiveExtendedIncludingNullableOrEnum())
                    {
                        //基元对象
                        IPropertyMapper pMap = new PropertyMapper(propertyInfo);
                        if (pMap.IsPrimaryKey)
                            pkeyProperties.Add(pMap);
                        else
                            otherProperties.Add(pMap);
                    }
                }
            }

            this.PKeyProperties = pkeyProperties.AsReadOnly();
            this.OtherProperties = otherProperties.AsReadOnly();
            this.DataObjectProperties = dataObjectProperties.AsReadOnly();
        }

        /// <summary>
        /// 获取IDataReader转Entity委托方法
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public DataReaderDeserializer GetDataReaderDeserializer(System.Data.IDataReader reader)
        {
            if (deserializer == null)
                deserializer = TypeDeserializerEmit.CreateDataReaderDeserializer(this, reader);
            return deserializer;
        }
    }
}
