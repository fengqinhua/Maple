namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// ���һ��ʵ��ʵ�ָýӿڣ���ô�贴�����޸ĺ�ɾ��ʵ����û���ʱ��
    /// </summary>
    public interface IFullAudited : IAudited, IDeletionAudited
    {
        
    }
}