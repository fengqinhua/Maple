using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maple.Core.Domain.Auditing
{
    /// <summary>
    /// 聚合根的基类，包含创建用户ID、创建时间、最近一次修改用户ID、最近一次修改时间信息、删除用户ID、删除时间、是否已删除等信息 （使用Long作为ID类型）
    /// </summary>
    [Serializable]
    public abstract class FullAuditedAggregateRoot : FullAuditedAggregateRoot<long>
    {

    }

    /// <summary>
    /// 集合根的基类，包含创建用户ID、创建时间、最近一次修改用户ID、最近一次修改时间信息、删除用户ID、删除时间、是否已删除等信息
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class FullAuditedAggregateRoot<TPrimaryKey> : AuditedAggregateRoot<TPrimaryKey>, IFullAudited
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

    /// <summary>
    /// 聚合根的基类，包含创建用户ID、创建时间、最近一次修改用户ID、最近一次修改时间信息、删除用户ID、删除时间、是否已删除等信息
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    [Serializable]
    public abstract class FullAuditedAggregateRoot<TPrimaryKey, TUser> : AuditedAggregateRoot<TPrimaryKey, TUser>, IFullAudited<TUser>
        where TUser : IEntity<long>
    {
        /// <summary>
        /// 是否已删除?
        /// </summary>
        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// 删除用户实体.
        /// </summary>
        [ForeignKey("DeleterUserId")]
        public virtual TUser DeleterUser { get; set; }

        /// <summary>
        /// 删除用户ID?
        /// </summary>
        public virtual long? DeleterUserId { get; set; }

        /// <summary>
        /// 删除时间.
        /// </summary>
        public virtual DateTime? DeletionTime { get; set; }
    }
}