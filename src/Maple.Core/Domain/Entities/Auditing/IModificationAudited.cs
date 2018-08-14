namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô��洢ʵ�����һ�ε��޸��û���ʱ��
    /// </summary>
    public interface IModificationAudited : IHasModificationTime
    {
        /// <summary>
        /// ʵ�����һ���޸ĵ��û�ID.
        /// </summary>
        long? LastModifierUserId { get; set; }
    }
}