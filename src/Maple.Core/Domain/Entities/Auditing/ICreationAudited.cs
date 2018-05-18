namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô�贴��ʵ����û���ʱ��
    /// </summary>
    public interface ICreationAudited : IHasCreationTime
    {
        /// <summary>
        ///  ����ʵ����û�ID.
        /// </summary>
        long? CreatorUserId { get; set; }
    }

    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô�贴��ʵ����û�ʵ��
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface ICreationAudited<TUser> : ICreationAudited
        where TUser : IEntity<long>
    {
        /// <summary>
        /// ����ʵ����û�ʵ��.
        /// </summary>
        TUser CreatorUser { get; set; }
    }
}