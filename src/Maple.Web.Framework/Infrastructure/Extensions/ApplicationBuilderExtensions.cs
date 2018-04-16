using Maple.Core.Infrastructure;
using Maple.Web.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// 扩展 IApplicationBuilder 的方法
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// MVC路由配置
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseMapleMvc(this IApplicationBuilder application)
        {
            application.UseMvc(routeBuilder =>
            {
                //register all routes
                EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(routeBuilder);
            });
        }
    }
}
