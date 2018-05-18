namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    ///  ���һ��ʵ��ʵ�ָýӿڣ���ô�贴�����޸�ʵ����û���ʱ��
    /// </summary>
    public interface IAudited : ICreationAudited, IModificationAudited
    {

    }

    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô�贴�����޸�ʵ����û���ʱ��
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface IAudited<TUser> : IAudited, ICreationAudited<TUser>, IModificationAudited<TUser>
        where TUser : IEntity<long>
    {

    }
}