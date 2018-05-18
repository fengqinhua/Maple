using System;

namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô��洢ʵ�����һ�ε��޸�ʱ��.
    /// </summary>
    public interface IHasModificationTime
    {
        /// <summary>
        /// ʵ������һ�ε��޸�ʱ��.
        /// </summary>
        DateTime? LastModificationTime { get; set; }
    }
}