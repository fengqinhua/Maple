using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// ʵ��Ļ��࣬���������û�ID������ʱ�䡢���һ���޸��û�ID�����һ���޸�ʱ����Ϣ ��Ĭ��ʹ��Long��ΪID��
    /// </summary>
    [Serializable]
    public abstract class AuditedEntity : AuditedEntity<long>, IEntity
    {

    }
    /// <summary>
    /// ʵ��Ļ��࣬���������û�ID������ʱ�䡢���һ���޸��û�ID�����һ���޸�ʱ����Ϣ
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class AuditedEntity<TPrimaryKey> : CreationAuditedEntity<TPrimaryKey>, IAudited
    {
        /// <summary>
        /// ���һ���޸�ʱ��.
        /// </summary>
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// ���һ���޸��û�ID.
        /// </summary>
        public virtual long? LastModifierUserId { get; set; }
    }

    /// <summary>
    /// ʵ��Ļ��࣬���������û�ID�������û�ʵ�塢����ʱ�䡢���һ���޸��û�ID�����һ���޸��û�ʵ�塢���һ���޸�ʱ����Ϣ
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    [Serializable]
    public abstract class AuditedEntity<TPrimaryKey, TUser> : AuditedEntity<TPrimaryKey>, IAudited<TUser>
        where TUser : IEntity<long>
    {
        /// <summary>
        /// �û�������ʵ��.
        /// </summary>
        [ForeignKey("CreatorUserId")]
        public virtual TUser CreatorUser { get; set; }

        /// <summary>
        /// �û��޸ĵ�ʵ��.
        /// </summary>
        [ForeignKey("LastModifierUserId")]
        public virtual TUser LastModifierUser { get; set; }
    }
}