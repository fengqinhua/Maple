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
    /// 代表应用程序启动时配置认证中间件的对象
    /// </summary>
    public class AuthenticationStartup : IMapleStartup
    {

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            //配置数据保护中涉及的密钥
            services.AddMapleDataProtection();

            //启用授权和认证服务
            services.AddMapleAuthentication();
        }

        public void Configure(IApplicationBuilder application)
        {
            //配置授权和认证服务
            application.UseMapleAuthentication();

            //set request culture
            application.UseCulture();
        }


        public int Order
        {
            get { return 500; }
        }
    }
}
