namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    ///  ���һ��ʵ��ʵ�ָýӿڣ���ô�贴�����޸�ʵ����û���ʱ��
    /// </summary>
    public interface IAudited : ICreationAudited, IModificationAudited
    {

    }
}