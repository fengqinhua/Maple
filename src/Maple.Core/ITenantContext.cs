using Maple.Core.Domain.Tenants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core
{
    /// <summary>
    /// 接口：站点上下文信息
    /// </summary>
    public interface ITenantContext
    {
        /// <summary>
        /// 获取当前请求对应的站点上下文信息
        /// </summary>
        Tenant CurrentTenant { get; }
    }
}
