using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// 接口：自定义MVC路由配置
    /// </summary>
    public interface IRoutePublisher
    {
        /// <summary>
        /// 设置MVC路由
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        void RegisterRoutes(IRouteBuilder routeBuilder);
    }
}
