using System;
using System.Collections.Generic;
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
        IList<IPropertyMapper> PKeyProperties { get; }

        /// <summary>
        /// 除标识列以外的属性集合
        /// </summary>
        IList<IPropertyMapper> OtherProperties { get; }
        /// <summary>
        /// 实体类的类型
        /// </summary>
        Type EntityType { get; }
    }

    public interface IEntityMapper<TEntity, TPrimaryKey> : IEntityMapper where TEntity : class, IEntity<TPrimaryKey> {

    }

}
