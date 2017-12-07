using System;
using System.ComponentModel.DataAnnotations.Schema;
using Maple.Core.Timing;

namespace Maple.Core.Domain.Auditing
{
    /// <summary>
    /// 实体的基类，包括创建该实体的用户ID及创建时间信息 （默认使用long作为实体的标识类型）
    /// </summary>
    [Serializable]
    public abstract class CreationAuditedEntity : CreationAuditedEntity<long>, IEntity
    {

    }

    /// <summary>
    /// 实体的基类，包括创建该实体的用户ID及创建时间信息
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class CreationAuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, ICreationAudited
    {
        /// <summary>
        /// 创建该实体的时间.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建该实体的用户ID.
        /// </summary>
        public virtual long? CreatorUserId { get; set; }

        /// <summary>
        /// 实体的基类，包括创建该实体的用户ID及创建时间信息.
        /// </summary>
        protected CreationAuditedEntity()
        {
            CreationTime = Clock.Now;
        }
    }

    /// <summary>
    /// 实体的基类，包括创建该实体的用户ID、用户实体及创建时间信息
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    [Serializable]
    public abstract class CreationAuditedEntity<TPrimaryKey, TUser> : CreationAuditedEntity<TPrimaryKey>, ICreationAudited<TUser>
        where TUser : IEntity<long>
    {
        /// <summary>
        /// 创建该实体的用户实体
        /// </summary>
        [ForeignKey("CreatorUserId")]
        public virtual TUser CreatorUser { get; set; }
    }
}