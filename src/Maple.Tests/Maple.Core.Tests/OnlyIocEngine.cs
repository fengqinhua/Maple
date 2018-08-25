using Autofac;
using Autofac.Extensions.DependencyInjection;
using Maple.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Tests
{
    public class OnlyIocEngine : IEngine
    {
        private IServiceProvider _serviceProvider { get; set; }
        public virtual IServiceProvider ServiceProvider => _serviceProvider;
        protected IServiceProvider GetServiceProvider()
        {
            return ServiceProvider;
            //var accessor = ServiceProvider.GetService<IHttpContextAccessor>();
            //var context = accessor.HttpContext;
            //return context != null ? context.RequestServices : ServiceProvider;
        }

        public OnlyIocEngine(Action<ContainerBuilder> build)
        {
            var containerBuilder = new ContainerBuilder();
            //注册IEngine的单例
            containerBuilder.RegisterInstance(this).As<IEngine>().SingleInstance();
            if (build != null)
                build(containerBuilder);

            ////注册可从程序集中发现各种类型的工具单例
            //containerBuilder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();
            ////查找实现了IDependencyRegistrar依赖注入接口的类
            //var dependencyRegistrars = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            ////按照Order排序并通过反射创建实现了IDependencyRegistrar依赖注入接口的类实例
            //var instances = dependencyRegistrars
            //    //.Where(dependencyRegistrar => PluginManager.FindPlugin(dependencyRegistrar).Return(plugin => plugin.Installed, true)) //ignore not installed plugins
            //    .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar))
            //    .OrderBy(dependencyRegistrar => dependencyRegistrar.Order);
            ////执行依赖注入
            //foreach (var dependencyRegistrar in instances)
            //    dependencyRegistrar.Register(containerBuilder, typeFinder, MapleConfig);
            ////将现有的Services路由到Autofac的管理集合中
            //containerBuilder.Populate(services);

            //创建基于Autofac的Asp.NET Core 中的服务提供者
            this._serviceProvider = new AutofacServiceProvider(containerBuilder.Build());
        }


        public void ConfigureRequestPipeline(IApplicationBuilder application)
        {
            throw new NotImplementedException();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            throw new NotImplementedException();
        }

        public void Initialize(IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>() where T : class
        {
            return (T)GetServiceProvider().GetRequiredService(typeof(T));
        }

        public object Resolve(Type type)
        {
            return GetServiceProvider().GetRequiredService(type);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>)GetServiceProvider().GetServices(typeof(T));
        }

        public object ResolveUnregistered(Type type)
        {
            throw new NotImplementedException();
        }

    }
}
