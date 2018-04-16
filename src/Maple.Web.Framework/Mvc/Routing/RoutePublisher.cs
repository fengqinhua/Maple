using Maple.Core.Infrastructure;
using Maple.Core.Plugins;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maple.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// 自定义MVC路由配置器
    /// </summary>
    public class RoutePublisher : IRoutePublisher
    {
        protected readonly ITypeFinder typeFinder;

        /// <summary>
        /// 自定义MVC路由配置器
        /// </summary>
        /// <param name="typeFinder"></param>
        public RoutePublisher(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
        }

        #region 方法

        /// <summary>
        /// 设置MVC路由
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public virtual void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //查找上下文中所有实现了IRouteProvider接口的类
            var routeProviders = typeFinder.FindClassesOfType<IRouteProvider>();

            //创造IRouteProvider实例并按照优先级排序
            var instances = routeProviders
                .Where(routeProvider => PluginManager.FindPlugin(routeProvider)?.Installed ?? true) //ignore not installed plugins
                .Select(routeProvider => (IRouteProvider)Activator.CreateInstance(routeProvider))
                .OrderByDescending(routeProvider => routeProvider.Priority);

            //逐个执行注册
            foreach (var routeProvider in instances)
                routeProvider.RegisterRoutes(routeBuilder);
        }

        #endregion
    }
}
