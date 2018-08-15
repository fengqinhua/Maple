using System;
using System.Collections.Generic;
using System.Reflection;
using Maple.Core.Domain.Entities;

namespace Maple.Core.Data.DbMappers
{
    /// <summary>
    /// 实体类IEntity与数据库对应的映射信息
    /// </summary>
    public interface IEntityMapper
    {
        /// <summary>
        /// 数据库架构名称
        /// </summary>
        string SchemaName { get; }
        /// <summary>
        /// 表名称
        /// </summary>
        string TableName { get; }
        /// <summary>
        /// 标识列对应属性集合（主键）
        /// </summary>
        IReadOnlyList<IPropertyMapper> PKeyProperties { get; }
        /// <summary>
        /// 除标识列以外的属性集合
        /// </summary>
        IReadOnlyList<IPropertyMapper> OtherProperties { get; }
        /// <summary>
        /// 值类型DataObject的属性集合
        /// </summary>
        IReadOnlyList<IDataObjectMapper> DataObjectProperties { get; }
        /// <summary>
        /// 实体类的类型
        /// </summary>
        Type EntityType { get; }
    }
}
