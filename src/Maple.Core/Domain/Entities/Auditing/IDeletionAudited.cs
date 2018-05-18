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

    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô��洢ɾ��ʵ����û���ʱ��
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface IDeletionAudited<TUser> : IDeletionAudited
        where TUser : IEntity<long>
    {
        /// <summary>
        /// ɾ��ʵ����û�ʵ��
        /// </summary>
        TUser DeleterUser { get; set; }
    }
}