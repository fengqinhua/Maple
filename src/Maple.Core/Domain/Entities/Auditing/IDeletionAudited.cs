namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// 如果一个实体实现该接口，那么需存储删除实体的用户及时间
    /// </summary>
    public interface IDeletionAudited : IHasDeletionTime
    {
        /// <summary>
        /// 删除实体的用户ID
        /// </summary>
        long? DeleterUserId { get; set; }
    }
}