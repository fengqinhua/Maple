using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// 实体的基类，包含创建用户ID、创建时间、最近一次修改用户ID、最近一次修改时间信息、删除用户ID、删除时间、是否已删除等信息 （使用Long作为ID类型）
    /// </summary>
    [Serializable]
    public abstract class FullAuditedEntity : FullAuditedEntity<long>, IEntity
    {

    }

    /// <summary>
    /// 实体的基类，包含创建用户ID、创建时间、最近一次修改用户ID、最近一次修改时间信息、删除用户ID、删除时间、是否已删除等信息
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class FullAuditedEntity<TPrimaryKey> : AuditedEntity<TPrimaryKey>, IFullAudited
    {
        /// <summary>
        /// 是否已删除?
        /// </summary>
        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// 删除用户ID
        /// </summary>
        public virtual long? DeleterUserId { get; set; }

        /// <summary>
        /// 删除时间.
        /// </summary>
        public virtual DateTime? DeletionTime { get; set; }
    }
}