using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// ʵ��Ļ��࣬���������û�ID������ʱ�䡢���һ���޸��û�ID�����һ���޸�ʱ����Ϣ��ɾ���û�ID��ɾ��ʱ�䡢�Ƿ���ɾ������Ϣ ��ʹ��Long��ΪID���ͣ�
    /// </summary>
    [Serializable]
    public abstract class FullAuditedEntity : FullAuditedEntity<long>, IEntity
    {

    }

    /// <summary>
    /// ʵ��Ļ��࣬���������û�ID������ʱ�䡢���һ���޸��û�ID�����һ���޸�ʱ����Ϣ��ɾ���û�ID��ɾ��ʱ�䡢�Ƿ���ɾ������Ϣ
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class FullAuditedEntity<TPrimaryKey> : AuditedEntity<TPrimaryKey>, IFullAudited
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
}