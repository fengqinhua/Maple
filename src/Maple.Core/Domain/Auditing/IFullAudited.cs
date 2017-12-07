namespace Maple.Core.Domain.Auditing
{
    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô�贴�����޸ĺ�ɾ��ʵ����û���ʱ��
    /// </summary>
    public interface IFullAudited : IAudited, IDeletionAudited
    {
        
    }

    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô�贴�����޸ĺ�ɾ��ʵ����û���ʱ��
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface IFullAudited<TUser> : IAudited<TUser>, IFullAudited, IDeletionAudited<TUser>
        where TUser : IEntity<long>
    {

    }
}