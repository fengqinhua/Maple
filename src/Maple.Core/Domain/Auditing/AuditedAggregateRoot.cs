using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maple.Core.Domain.Auditing
{
    /// <summary>
    /// �ۺϸ��Ļ��࣬���������û�ID������ʱ�䡢���һ���޸��û�ID�����һ���޸�ʱ����Ϣ ��Ĭ��ʹ��Long��ΪID��
    /// </summary>
    [Serializable]
    public abstract class AuditedAggregateRoot : AuditedAggregateRoot<long>
    {

    }

    /// <summary>
    /// �ۺϸ��Ļ��࣬���������û�ID������ʱ�䡢���һ���޸��û�ID�����һ���޸�ʱ����Ϣ
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class AuditedAggregateRoot<TPrimaryKey> : CreationAuditedAggregateRoot<TPrimaryKey>, IAudited
    {
        /// <summary>
        /// ʵ�����һ���޸ĵ�ʱ��.
        /// </summary>
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// ʵ�����һ���޸ĵ��û�ID.
        /// </summary>
        public virtual long? LastModifierUserId { get; set; }
    }

    /// <summary>
    /// �ۺϸ��Ļ��࣬���������û�ID�������û�ʵ�塢����ʱ�䡢���һ���޸��û�ID�����һ���޸��û�ʵ�塢���һ���޸�ʱ����Ϣ
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    [Serializable]
    public abstract class AuditedAggregateRoot<TPrimaryKey, TUser> : AuditedAggregateRoot<TPrimaryKey>, IAudited<TUser>
        where TUser : IEntity<long>
    {
        /// <summary>
        /// ������ʵ����û�ʵ��.
        /// </summary>
        [ForeignKey("CreatorUserId")]
        public virtual TUser CreatorUser { get; set; }

        /// <summary>
        /// �޸ĸ�ʵ����û�ʵ��.
        /// </summary>
        [ForeignKey("LastModifierUserId")]
        public virtual TUser LastModifierUser { get; set; }
    }
}