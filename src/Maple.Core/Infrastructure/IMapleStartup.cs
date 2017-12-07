using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Infrastructure
{
    /// <summary>
    /// 接口：表示在应用程序启动时配置服务和中间件的对象
    /// </summary>
    public interface IMapleStartup
    {
        /// <summary>
        /// 添加和配置中间件
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration);

        /// <summary>
        /// 配置已添加的中间件
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        void Configure(IApplicationBuilder application);

        /// <summary>
        /// 顺序
        /// </summary>
        int Order { get; }
    }
}
