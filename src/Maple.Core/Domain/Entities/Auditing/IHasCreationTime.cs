using System;

namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô��洢ʵ��Ĵ���ʱ��
    /// </summary>
    public interface IHasCreationTime
    {
        /// <summary>
        /// ʵ��Ĵ���ʱ��
        /// </summary>
        DateTime CreationTime { get; set; }
    }
}