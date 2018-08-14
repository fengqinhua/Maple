namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô��洢ɾ��ʵ����û���ʱ��
    /// </summary>
    public interface IDeletionAudited : IHasDeletionTime
    {
        /// <summary>
        /// ɾ��ʵ����û�ID
        /// </summary>
        long? DeleterUserId { get; set; }
    }
}