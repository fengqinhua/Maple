using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maple.Core.Domain.Tenants
{
    /// <summary>
    /// Tenant扩展方法
    /// </summary>
    public static class TenantExtensions
    {
        /// <summary>
        /// 获得站点对应的Host集合
        /// </summary>
        /// <param name="tenant">tenant</param>
        /// <returns>Comma-separated hosts</returns>
        public static string[] ParseHostValues(this Tenant tenant)
        {
            Check.NotNull(tenant, nameof(tenant));

            var parsedValues = new List<string>();
            if (!string.IsNullOrEmpty(tenant.Hosts))
            {
                var hosts = tenant.Hosts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var host in hosts)
                {
                    var tmp = host.Trim();
                    if (!string.IsNullOrEmpty(tmp))
                        parsedValues.Add(tmp);
                }
            }
            return parsedValues.ToArray();
        }

        /// <summary>
        /// 判断站点中是否包含指定的Host
        /// </summary>
        /// <param name="tenant">tenant</param>
        /// <param name="host">Host</param>
        /// <returns>true - contains, false - no</returns>
        public static bool ContainsHostValue(this Tenant tenant, string host)
        {
            Check.NotNull(tenant, nameof(tenant));

            if (string.IsNullOrEmpty(host))
                return false;

            var contains = tenant.ParseHostValues()
                                .FirstOrDefault(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase)) != null;
            return contains;
        }
    }
}
