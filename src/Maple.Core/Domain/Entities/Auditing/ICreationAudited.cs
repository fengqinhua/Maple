namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// 如果一个实体实现该接口，那么需创建实体的用户及时间
    /// </summary>
    public interface ICreationAudited : IHasCreationTime
    {
        /// <summary>
        ///  创建实体的用户ID.
        /// </summary>
        long? CreatorUserId { get; set; }
    }
}