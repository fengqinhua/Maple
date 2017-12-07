using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maple.Core.Domain.Auditing
{
    /// <summary>
    /// 聚合根的基类，包含创建用户ID、创建时间、最近一次修改用户ID、最近一次修改时间信息 （默认使用Long作为ID）
    /// </summary>
    [Serializable]
    public abstract class AuditedAggregateRoot : AuditedAggregateRoot<long>
    {

    }

    /// <summary>
    /// 聚合根的基类，包含创建用户ID、创建时间、最近一次修改用户ID、最近一次修改时间信息
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class AuditedAggregateRoot<TPrimaryKey> : CreationAuditedAggregateRoot<TPrimaryKey>, IAudited
    {
        /// <summary>
        /// 实体最后一次修改的时间.
        /// </summary>
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 实体最后一次修改的用户ID.
        /// </summary>
        public virtual long? LastModifierUserId { get; set; }
    }

    /// <summary>
    /// 聚合根的基类，包含创建用户ID、创建用户实体、创建时间、最近一次修改用户ID、最近一次修改用户实体、最近一次修改时间信息
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    [Serializable]
    public abstract class AuditedAggregateRoot<TPrimaryKey, TUser> : AuditedAggregateRoot<TPrimaryKey>, IAudited<TUser>
        where TUser : IEntity<long>
    {
        /// <summary>
        /// 创建该实体的用户实体.
        /// </summary>
        [ForeignKey("CreatorUserId")]
        public virtual TUser CreatorUser { get; set; }

        /// <summary>
        /// 修改该实体的用户实体.
        /// </summary>
        [ForeignKey("LastModifierUserId")]
        public virtual TUser LastModifierUser { get; set; }
    }
}