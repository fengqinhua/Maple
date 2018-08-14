using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// 实体的基类，包含创建用户ID、创建时间、最近一次修改用户ID、最近一次修改时间信息 （默认使用Long作为ID）
    /// </summary>
    [Serializable]
    public abstract class AuditedEntity : AuditedEntity<long>, IEntity
    {

    }
    /// <summary>
    /// 实体的基类，包含创建用户ID、创建时间、最近一次修改用户ID、最近一次修改时间信息
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class AuditedEntity<TPrimaryKey> : CreationAuditedEntity<TPrimaryKey>, IAudited
    {
        /// <summary>
        /// 最近一次修改时间.
        /// </summary>
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 最近一次修改用户ID.
        /// </summary>
        public virtual long? LastModifierUserId { get; set; }
    }
}