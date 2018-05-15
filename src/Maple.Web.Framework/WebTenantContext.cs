using Maple.Core;
using Maple.Core.Domain.Tenants;
using Maple.Services.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maple.Web.Framework
{
    /// <summary>
    /// 站点上下文信息
    /// </summary>
    public partial class WebTenantContext : ITenantContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantService _tenantService;

        private Tenant _cachedTenant;


        public WebTenantContext(IHttpContextAccessor httpContextAccessor,
            ITenantService tenantService)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._tenantService = tenantService;
        }

        /// <summary>
        /// 获取当前请求对应的站点上下文信息
        /// </summary>
        public virtual Tenant CurrentTenant
        {
            get
            {
                if (_cachedTenant != null)
                    return _cachedTenant;

                //try to determine the current store by HOST header
                string host = _httpContextAccessor.HttpContext?.Request?.Headers[HeaderNames.Host];

                var allTenants = _tenantService.GetAll();
                var tenant = allTenants.FirstOrDefault(s => s.ContainsHostValue(host));

                if (tenant == null)
                {
                    //load the first found store
                    tenant = allTenants.FirstOrDefault();
                }

                _cachedTenant = tenant ?? throw new Exception("No store could be loaded");
                return _cachedTenant;
            }
        }
    }
}
