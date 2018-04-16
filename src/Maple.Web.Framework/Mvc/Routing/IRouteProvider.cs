using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// 接口 MVC路由提供者
    /// </summary>
    public interface IRouteProvider
    {
        /// <summary>
        /// 注册路由
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        void RegisterRoutes(IRouteBuilder routeBuilder);

        /// <summary>
        /// 标识优先级的值
        /// </summary>
        int Priority { get; }
    }
}
