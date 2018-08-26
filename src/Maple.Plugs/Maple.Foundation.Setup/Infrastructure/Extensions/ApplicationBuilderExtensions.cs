using Maple.Foundation.Setup.Infrastructure.Http;
using Microsoft.AspNetCore.Builder;

namespace Maple.Foundation.Setup.Infrastructure.Extensions
{
    /// <summary>
    /// 扩展 IApplicationBuilder 的方法
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 配置中间件 检测数据是否已经完成配置
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseInstallUrl(this IApplicationBuilder application)
        {
            application.UseMiddleware<InstallUrlMiddleware>();
        }
    }
}
