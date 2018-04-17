using Maple.Web.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Foundation.Setup.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(SetupFoundationDefaluts.InstallRoute,
                "plugins/install", new { controller = "Install", action = "Index" });
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
