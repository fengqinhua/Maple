using System;
using System.ComponentModel.DataAnnotations.Schema;
using Maple.Core.Timing;

namespace Maple.Core.Domain.Auditing
{
    /// <summary>
    /// 集合根的基类，包含创建该集合根的用户ID及创建时间信息（默认使用long作为聚合跟标识）
    /// </summary>
    [Serializable]
    public abstract class CreationAuditedAggregateRoot : CreationAuditedAggregateRoot<long>
    {
        
    }

    /// <summary>
    /// 集合根的基类，包含创建该集合根的用户ID及创建时间信息
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class CreationAuditedAggregateRoot<TPrimaryKey> : AggregateRoot<TPrimaryKey>, ICreationAudited
    {
        /// <summary>
        /// 聚合根的创建时间
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// 聚合根的创建用户ID.
        /// </summary>
        public virtual long? CreatorUserId { get; set; }

        /// <summary>
        /// 集合根的基类，包含创建该集合根的用户及创建时间信息.
        /// </summary>
        protected CreationAuditedAggregateRoot()
        {
            CreationTime = Clock.Now;
        }
    }

    /// <summary>
    /// 集合根的基类，包含创建该集合根的用户ID、用户实体、创建时间信息
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    [Serializable]
    public abstract class CreationAuditedAggregateRoot<TPrimaryKey, TUser> : CreationAuditedAggregateRoot<TPrimaryKey>, ICreationAudited<TUser>
        where TUser : IEntity<long>
    {
        /// <summary>
        /// 聚合根的创建用户实体
        /// </summary>
        [ForeignKey("CreatorUserId")]
        public virtual TUser CreatorUser { get; set; }
    }
}
