namespace Maple.Core.Domain.Auditing
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

    /// <summary>
    /// 如果一个实体实现该接口，那么需创建实体的用户实体
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface ICreationAudited<TUser> : ICreationAudited
        where TUser : IEntity<long>
    {
        /// <summary>
        /// 创建实体的用户实体.
        /// </summary>
        TUser CreatorUser { get; set; }
    }
}