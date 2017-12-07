using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Maple.Core.Configuration;
using Maple.Core.Infrastructure.DependencyManagement;
using Maple.Core.Infrastructure.Mapper;
using Maple.Core.Plugins;

namespace Maple.Core.Infrastructure
{
    /// <summary>
    /// Represents Nop engine
    /// </summary>
    public class MapleEngine : IEngine
    {
        #region 属性

        /// <summary>
        /// Asp.NET Core 中的服务提供者
        /// </summary>
        private IServiceProvider _serviceProvider { get; set; }

        #endregion

        #region 非共有方法

        /// <summary>
        /// 获取Asp.NET Core 中的服务提供者
        /// </summary>
        /// <returns>IServiceProvider</returns>
        protected IServiceProvider GetServiceProvider()
        {
            var accessor = ServiceProvider.GetService<IHttpContextAccessor>();
            var context = accessor.HttpContext;
            return context != null ? context.RequestServices : ServiceProvider;
        }

        /// <summary>
        /// 运行 startup 启动时的任务
        /// </summary>
        /// <param name="typeFinder">Type finder</param>
        protected virtual void RunStartupTasks(ITypeFinder typeFinder)
        {
            //查找实现了IStartupTask的类型
            var startupTasks = typeFinder.FindClassesOfType<IStartupTask>();

            //反射创建IStartupTask的类型并按照Order顺序执行
            var instances = startupTasks
                .Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask))
                .OrderBy(startupTask => startupTask.Order);

            //执行任务
            foreach (var task in instances)
                task.Execute();
        }

        /// <summary>
        /// 使用 Autofac 重新注册依赖关系
        /// </summary>
        /// <param name="MapleConfig">Startup Nop configuration parameters</param>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="typeFinder">Type finder</param>
        protected virtual IServiceProvider RegisterDependencies(MapleConfig MapleConfig, IServiceCollection services, ITypeFinder typeFinder)
        {
            var containerBuilder = new ContainerBuilder();
            //注册IEngine的单例
            containerBuilder.RegisterInstance(this).As<IEngine>().SingleInstance();
            //注册可从程序集中发现各种类型的工具单例
            containerBuilder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();
            //查找实现了IDependencyRegistrar依赖注入接口的类
            var dependencyRegistrars = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            //按照Order排序并通过反射创建实现了IDependencyRegistrar依赖注入接口的类实例
            var instances = dependencyRegistrars
                //.Where(dependencyRegistrar => PluginManager.FindPlugin(dependencyRegistrar).Return(plugin => plugin.Installed, true)) //ignore not installed plugins
                .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar))
                .OrderBy(dependencyRegistrar => dependencyRegistrar.Order);
            //执行依赖注入
            foreach (var dependencyRegistrar in instances)
                dependencyRegistrar.Register(containerBuilder, typeFinder, MapleConfig);
            //将现有的Services路由到Autofac的管理集合中
            containerBuilder.Populate(services);
            //创建基于Autofac的Asp.NET Core 中的服务提供者
            _serviceProvider = new AutofacServiceProvider(containerBuilder.Build());
            return _serviceProvider;
        }

        /// <summary>
        /// 注册并配置AutoMapper
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="typeFinder">Type finder</param>
        protected virtual void AddAutoMapper(IServiceCollection services, ITypeFinder typeFinder)
        {
            //查找其他程序集提供的AutoMapper映射器配置
            var mapperConfigurations = typeFinder.FindClassesOfType<IMapperProfile>();

            //确保AutoMapper映射器所在的插件均已被加载
            //按照Order排序并通过反射创建IMapperProfile的实例
            var instances = mapperConfigurations
                .Where(mapperConfiguration => PluginManager.FindPlugin(mapperConfiguration)?.Installed ?? true) //ignore not installed plugins
                .Select(mapperConfiguration => (IMapperProfile)Activator.CreateInstance(mapperConfiguration))
                .OrderBy(mapperConfiguration => mapperConfiguration.Order);
            //添加AutoMapper配置
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var instance in instances)
                {
                    cfg.AddProfile(instance.GetType());
                }
            });
            //添加依赖注入
            services.AddAutoMapper();
            //注册
            AutoMapperConfiguration.Init(config);
        }

        #endregion

        #region 共有方法

        /// <summary>
        /// 初始化Maple引擎
        /// <para>1、指定安全协议</para>
        /// <para>2、设置应用程序根目录</para>
        /// <para>3、初始化插件</para>
        /// </summary>
        /// <param name="services"></param>
        public void Initialize(IServiceCollection services)
        {
            //most of API providers require TLS 1.2 nowadays
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //获取配置信息
            var provider = services.BuildServiceProvider();
            //获得在托管应用程序的应用程序域内向托管应用程序提供应用程序管理功能和应用程序服务实例
            var hostingEnvironment = provider.GetRequiredService<IHostingEnvironment>();
            //获得MapleConfig实例
            var MapleConfig = provider.GetRequiredService<MapleConfig>();
            //设置应用程序根目录
            CommonHelper.BaseDirectory = hostingEnvironment.ContentRootPath;
            //初始化插件
            var mvcCoreBuilder = services.AddMvcCore();
            PluginManager.Initialize(mvcCoreBuilder.PartManager, MapleConfig);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //检查程序集是否已经加载了
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;

            //返回Maple.Core 程序集
            var tf = Resolve<ITypeFinder>();
            assembly = tf.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            return assembly;
        }

        /// <summary>
        /// 添加配置服务
        /// <para>1、查询所有实现了IMapleStartup类的实例，并确保其所在插件均已被加载</para>
        /// <para>2、按顺序执行中间件的添加和配置</para>
        /// <para>3、注册并配置AutoMapper</para>
        /// <para>4、使用 Autofac 重新注册依赖关系</para>
        /// <para>5、运行 startup 启动时的任务</para>
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        /// <returns>Service provider</returns>
        public IServiceProvider ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            //查找其他程序集提供的启动配置
            var typeFinder = new WebAppTypeFinder();
            var startupConfigurations = typeFinder.FindClassesOfType<IMapleStartup>();

            //确保IMapleStartup实现类所在的插件均已被加载
            //按照Order排序并通过反射创建IMapleStartup实现类的实例
            var instances = startupConfigurations
                .Where(startup => PluginManager.FindPlugin(startup)?.Installed ?? true) //ignore not installed plugins
                .Select(startup => (IMapleStartup)Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);

            //执行中间件的添加与配置
            foreach (var instance in instances)
                instance.ConfigureServices(services, configuration);

            //注册并配置AutoMapper
            AddAutoMapper(services, typeFinder);

            //使用 Autofac 重新注册依赖关系
            var MapleConfig = services.BuildServiceProvider().GetService<MapleConfig>();
            RegisterDependencies(MapleConfig, services, typeFinder);

            //运行 startup 启动时的任务
            if (!MapleConfig.IgnoreStartupTasks)
                RunStartupTasks(typeFinder);

            //注册程序集的解析失败时的事件
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            //set App_Data path as base data directory (required to create and save SQL Server Compact database file in App_Data folder)
            AppDomain.CurrentDomain.SetData("DataDirectory", CommonHelper.MapPath("~/App_Data/"));

            return _serviceProvider;
        }

        /// <summary>
        /// 配置 HTTP 请求管道
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void ConfigureRequestPipeline(IApplicationBuilder application)
        {
            //获得可从程序集中发现各种类型的工具实例
            var typeFinder = Resolve<ITypeFinder>();
            //查找其他程序集提供的启动配置
            var startupConfigurations = typeFinder.FindClassesOfType<IMapleStartup>();
            //确保IMapleStartup实现类所在的插件均已被加载
            //按照Order排序并通过反射创建IMapleStartup实现类的实例
            var instances = startupConfigurations
                .Where(startup => PluginManager.FindPlugin(startup)?.Installed ?? true) //ignore not installed plugins
                .Select(startup => (IMapleStartup)Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);
            //执行中间件配置
            foreach (var instance in instances)
                instance.Configure(application);
        }

        /// <summary>
        /// 获取指定类型的对象实例
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        public T Resolve<T>() where T : class
        {
            return (T)GetServiceProvider().GetRequiredService(typeof(T));
        }

        /// <summary>
        /// 获取指定类型的对象实例
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Resolved service</returns>
        public object Resolve(Type type)
        {
            return GetServiceProvider().GetRequiredService(type);
        }

        /// <summary>
        /// 获取指定类型的对象实例集合
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        public IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>)GetServiceProvider().GetServices(typeof(T));
        }

        /// <summary>
        /// 获取未注册的对象实例
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <returns>Resolved service</returns>
        public virtual object ResolveUnregistered(Type type)
        {
            Exception innerException = null;
            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    //try to resolve constructor parameters
                    var parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null)
                            throw new MapleException("Unknown dependency");
                        return service;
                    });

                    //all is ok, so create instance
                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }
            throw new MapleException("没有找到满足所有依赖项的构造函数.", innerException);
        }

        #endregion

        #region 字段

        /// <summary>
        /// Asp.NET Core 中的服务提供者
        /// </summary>
        public virtual IServiceProvider ServiceProvider => _serviceProvider;

        #endregion
    }
}
