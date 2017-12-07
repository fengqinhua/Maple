using System;
using System.ComponentModel.DataAnnotations.Schema;
using Maple.Core.Timing;

namespace Maple.Core.Domain.Auditing
{
    /// <summary>
    /// ʵ��Ļ��࣬����������ʵ����û�ID������ʱ����Ϣ ��Ĭ��ʹ��long��Ϊʵ��ı�ʶ���ͣ�
    /// </summary>
    [Serializable]
    public abstract class CreationAuditedEntity : CreationAuditedEntity<long>, IEntity
    {

    }

    /// <summary>
    /// ʵ��Ļ��࣬����������ʵ����û�ID������ʱ����Ϣ
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class CreationAuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, ICreationAudited
    {
        /// <summary>
        /// ������ʵ���ʱ��.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// ������ʵ����û�ID.
        /// </summary>
        public virtual long? CreatorUserId { get; set; }

        /// <summary>
        /// ʵ��Ļ��࣬����������ʵ����û�ID������ʱ����Ϣ.
        /// </summary>
        protected CreationAuditedEntity()
        {
            CreationTime = Clock.Now;
        }
    }

    /// <summary>
    /// ʵ��Ļ��࣬����������ʵ����û�ID���û�ʵ�弰����ʱ����Ϣ
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    [Serializable]
    public abstract class CreationAuditedEntity<TPrimaryKey, TUser> : CreationAuditedEntity<TPrimaryKey>, ICreationAudited<TUser>
        where TUser : IEntity<long>
    {
        /// <summary>
        /// ������ʵ����û�ʵ��
        /// </summary>
        [ForeignKey("CreatorUserId")]
        public virtual TUser CreatorUser { get; set; }
    }
}