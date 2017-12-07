﻿namespace Maple.Core.Domain
{
    /// <summary>
    /// 如果一个实体实现该接口，那么必须存储实体所在组织的ID.
    /// </summary>
    public interface IMustHaveOrg
    {
        /// <summary>
        /// 实体所在组织ID.
        /// </summary>
        int OrgId { get; set; }
    }
}