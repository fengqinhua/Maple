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

    /// <summary>
    /// 实体的基类，包含创建用户ID、创建用户实体、创建时间、最近一次修改用户ID、最近一次修改用户实体、最近一次修改时间信息
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    [Serializable]
    public abstract class AuditedEntity<TPrimaryKey, TUser> : AuditedEntity<TPrimaryKey>, IAudited<TUser>
        where TUser : IEntity<long>
    {
        /// <summary>
        /// 用户创建的实体.
        /// </summary>
        [ForeignKey("CreatorUserId")]
        public virtual TUser CreatorUser { get; set; }

        /// <summary>
        /// 用户修改的实体.
        /// </summary>
        [ForeignKey("LastModifierUserId")]
        public virtual TUser LastModifierUser { get; set; }
    }
}