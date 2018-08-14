namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// 如果一个实体实现该接口，那么需存储实体最近一次的修改用户及时间
    /// </summary>
    public interface IModificationAudited : IHasModificationTime
    {
        /// <summary>
        /// 实体最近一次修改的用户ID.
        /// </summary>
        long? LastModifierUserId { get; set; }
    }
}