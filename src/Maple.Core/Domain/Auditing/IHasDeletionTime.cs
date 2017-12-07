using System;

namespace Maple.Core.Domain.Auditing
{
    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô��洢ʵ���ɾ��ʱ��
    /// </summary>
    public interface IHasDeletionTime : ISoftDelete
    {
        /// <summary>
        /// ʵ���ɾ��ʱ��.
        /// </summary>
        DateTime? DeletionTime { get; set; }
    }
}