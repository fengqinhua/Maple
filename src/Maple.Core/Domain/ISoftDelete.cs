namespace Maple.Core.Domain
{
    /// <summary>
    /// 接口 ： 标识实体是否已经被软删除.
    /// </summary>
    public interface ISoftDelete
    {
        /// <summary>
        /// 标识实体是否已经被软删除. 
        /// </summary>
        bool IsDeleted { get; set; }
    }
}
