using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Maple.Web.Framework.Infrastructure.Extensions;

namespace Maple.Web.App
{
    /// <summary>
    /// Represents startup class of application
    /// </summary>
    public class Startup
    {
        #region Properties

        /// <summary>
        /// Get configuration root of the application
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        #endregion

        #region Ctor

        public Startup(IHostingEnvironment environment)
        {
            //这里创建ConfigurationBuilder，其作用就是加载Congfig等配置文件
            Configuration = new ConfigurationBuilder()
                //将项目的跟目录设置为ConfigurationBuilder的跟路径
                .SetBasePath(environment.ContentRootPath)
                //使用AddJsonFile方法把项目中的appsettings.json配置文件加载进来，后面的reloadOnChange顾名思义就是文件如果改动就重新加载
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                //将系统的环境变量也加载进来
                .AddEnvironmentVariables()
                .Build();
        }

        #endregion

        /// <summary>
        /// 注册对象
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.ConfigureApplicationServices(Configuration);
        }

        /// <summary>
        /// 注册HTTP管道
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //application.ConfigureRequestPipeline();
        }
    }
}
