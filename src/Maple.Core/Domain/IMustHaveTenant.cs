using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain
{
    /// <summary>
    /// 如果一个实体实现该接口，那么必须存储实体所在站点的ID.
    /// </summary>
    public interface IMustHaveTenant
    {
        /// <summary>
        /// 站点标识
        /// </summary>
        long TenantId { get; set; }
    }
}
