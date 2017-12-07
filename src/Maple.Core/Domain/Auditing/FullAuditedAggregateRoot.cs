using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maple.Core.Domain.Auditing
{
    /// <summary>
    /// �ۺϸ��Ļ��࣬���������û�ID������ʱ�䡢���һ���޸��û�ID�����һ���޸�ʱ����Ϣ��ɾ���û�ID��ɾ��ʱ�䡢�Ƿ���ɾ������Ϣ ��ʹ��Long��ΪID���ͣ�
    /// </summary>
    [Serializable]
    public abstract class FullAuditedAggregateRoot : FullAuditedAggregateRoot<long>
    {

    }

    /// <summary>
    /// ���ϸ��Ļ��࣬���������û�ID������ʱ�䡢���һ���޸��û�ID�����һ���޸�ʱ����Ϣ��ɾ���û�ID��ɾ��ʱ�䡢�Ƿ���ɾ������Ϣ
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class FullAuditedAggregateRoot<TPrimaryKey> : AuditedAggregateRoot<TPrimaryKey>, IFullAudited
    {
        /// <summary>
        /// �Ƿ���ɾ��?
        /// </summary>
        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// ɾ���û�ID
        /// </summary>
        public virtual long? DeleterUserId { get; set; }

        /// <summary>
        /// ɾ��ʱ��.
        /// </summary>
        public virtual DateTime? DeletionTime { get; set; }
    }

    /// <summary>
    /// �ۺϸ��Ļ��࣬���������û�ID������ʱ�䡢���һ���޸��û�ID�����һ���޸�ʱ����Ϣ��ɾ���û�ID��ɾ��ʱ�䡢�Ƿ���ɾ������Ϣ
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    [Serializable]
    public abstract class FullAuditedAggregateRoot<TPrimaryKey, TUser> : AuditedAggregateRoot<TPrimaryKey, TUser>, IFullAudited<TUser>
        where TUser : IEntity<long>
    {
        /// <summary>
        /// �Ƿ���ɾ��?
        /// </summary>
        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// ɾ���û�ʵ��.
        /// </summary>
        [ForeignKey("DeleterUserId")]
        public virtual TUser DeleterUser { get; set; }

        /// <summary>
        /// ɾ���û�ID?
        /// </summary>
        public virtual long? DeleterUserId { get; set; }

        /// <summary>
        /// ɾ��ʱ��.
        /// </summary>
        public virtual DateTime? DeletionTime { get; set; }
    }
}