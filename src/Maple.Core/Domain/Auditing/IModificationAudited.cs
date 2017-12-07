namespace Maple.Core.Domain.Auditing
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

    /// <summary>
    /// 如果一个实体实现该接口，那么需存储实体最近一次的修改用户及时间
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface IModificationAudited<TUser> : IModificationAudited
        where TUser : IEntity<long>
    {
        /// <summary>
        /// 实体最近一次修改的用户实体.
        /// </summary>
        TUser LastModifierUser { get; set; }
    }
}