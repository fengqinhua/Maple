using Maple.Core.Infrastructure;
using Maple.Web.Framework.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Web.Framework.Infrastructure
{
    /// <summary>
    /// 用于在应用程序启动时执行异常处理相关的配置
    /// </summary>
    public class ErrorHandlerStartup : IMapleStartup
    {

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
        }


        public void Configure(IApplicationBuilder application)
        {
            //exception handling
            application.UseMapleExceptionHandler();

            //handle 400 errors (bad request)
            application.UseBadRequestResult();

            //handle 404 errors (not found)
            application.UsePageNotFound();
        }

        public int Order
        {
            //异常处理的设置应该最先加载
            get { return 0; }
        }
    }
}
