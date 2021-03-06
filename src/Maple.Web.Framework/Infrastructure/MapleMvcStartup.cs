﻿using Maple.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Maple.Web.Framework.Infrastructure.Extensions;

namespace Maple.Web.Framework.Infrastructure
{
    /// <summary>
    /// 用于在应用程序启动时执行MVC相关配置
    /// </summary>
    public class MapleMvcStartup : IMapleStartup
    {
        public int Order => 1000;

        public void Configure(IApplicationBuilder application)
        {
            //???
            ////add MiniProfiler
            //application.UseMiniProfiler();

            //MVC 路由配置
            application.UseMapleMvc();
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            //???
            ////add MiniProfiler services
            //services.AddMiniProfiler();

            //为应用程序添加和配置MVC
            services.AddMapleMvc();
        }
    }
}
