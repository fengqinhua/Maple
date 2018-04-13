using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Infrastructure
{
    /// <summary>
    /// Maple 引擎
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// 初始化 Maple 引擎
        /// <para>1、指定安全协议</para>
        /// <para>2、设置应用程序根目录</para>
        /// <para>3、初始化插件</para>
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        void Initialize(IServiceCollection services);

        /// <summary>
        /// 在DI容器中注册服务
        /// <para>1、查询所有实现了IMapleStartup类的实例，并确保其所在插件均已被加载</para>
        /// <para>2、按顺序执行中间件的添加和配置</para>
        /// <para>3、注册并配置AutoMapper</para>
        /// <para>4、使用 Autofac 重新注册依赖关系</para>
        /// <para>5、运行 startup 启动时的任务</para>
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        /// <returns>Service provider</returns>
        IServiceProvider ConfigureServices(IServiceCollection services, IConfigurationRoot configuration);

        /// <summary>
        /// 配置HTTP请求管道
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        void ConfigureRequestPipeline(IApplicationBuilder application);

        /// <summary>
        /// 通过依赖注入创建对象
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// 通过依赖注入创建对象
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Resolved service</returns>
        object Resolve(Type type);

        /// <summary>
        /// 通过依赖注入创建对象
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        IEnumerable<T> ResolveAll<T>();

        /// <summary>
        /// 创建未注册的对象
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <returns>Resolved service</returns>
        object ResolveUnregistered(Type type);
    }
}
