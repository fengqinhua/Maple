namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// 如果一个实体实现该接口，那么需创建、修改和删除实体的用户及时间
    /// </summary>
    public interface IFullAudited : IAudited, IDeletionAudited
    {
        
    }
}