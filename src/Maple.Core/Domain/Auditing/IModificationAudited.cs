namespace Maple.Core.Domain.Auditing
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

    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô��洢ʵ�����һ�ε��޸��û���ʱ��
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface IModificationAudited<TUser> : IModificationAudited
        where TUser : IEntity<long>
    {
        /// <summary>
        /// ʵ�����һ���޸ĵ��û�ʵ��.
        /// </summary>
        TUser LastModifierUser { get; set; }
    }
}