namespace Maple.Core.Domain.Auditing
{
    /// <summary>
    /// 如果一个实体实现该接口，那么需创建、修改和删除实体的用户及时间
    /// </summary>
    public interface IFullAudited : IAudited, IDeletionAudited
    {
        
    }

    /// <summary>
    /// 如果一个实体实现该接口，那么需创建、修改和删除实体的用户及时间
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface IFullAudited<TUser> : IAudited<TUser>, IFullAudited, IDeletionAudited<TUser>
        where TUser : IEntity<long>
    {

    }
}