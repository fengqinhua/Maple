namespace Maple.Core.Domain
{ 
    /// <summary>
    /// 如果一个实体实现该接口，那么可能会存储实体所在组织的ID.
    /// </summary>
    public interface IMayHaveOrg
    {
        /// <summary>
        /// 实体所在组织ID.
        /// </summary>
        int? OrgId { get; set; }
    }
}