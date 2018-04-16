using Maple.Core;
using Maple.Core.Configuration;
using Maple.Core.Data;
using Maple.Core.Infrastructure;
using Maple.Core.Plugins;
using Maple.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Maple.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// 扩展IServiceCollection的方法
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 向应用程序添加服务并配置服务提供程序
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceProvider ConfigureApplicationServices(this IServiceCollection services, IConfigurationRoot configuration)
        {
            //获取MapleConfig配置参数并将其以单例形式注入至IServiceCollection
            //MapleConfig = 系统配置信息
            services.ConfigureStartupConfig<MapleConfig>(configuration.GetSection("Maple"));
            //获取宿主程序运行环境所需的配置参数并将其以单例形式注入至IServiceCollection
            //HostingConfig = 宿主机的配置信息
            services.ConfigureStartupConfig<HostingConfig>(configuration.GetSection("Hosting"));
            //注册 HttpContextAccessor 单例 ，用于获取 HttpContext.Current
            services.AddHttpContextAccessor();

            //创建 Maple 引擎
            var engine = EngineContext.Create();
            //初始化 Maple 引擎  [ 指定安全协议 \ 设置应用程序根目录 \ 初始化插件 ]
            engine.Initialize(services);
            //在DI容器中注册服务 [ 查询所有实现了IMapleStartup类的实例，并确保其所在插件均已被加载 \ 按顺序执行中间件的添加和配置 \ 
            //                     注册并配置AutoMapper \ 使用 Autofac 重新注册依赖关系 \ 运行 startup 启动时的任务 ]
            var serviceProvider = engine.ConfigureServices(services, configuration);

            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                ////如果数据库已完成配置和安装，那么开始执行计划任务
                //TaskManager.Instance.Initialize();
                //TaskManager.Instance.Start();

                //标识用于程序已启动成功
                EngineContext.Current.Resolve<ILogger<IServiceCollection>>().LogInformation("Maple Application 已启动...");
            }
            return serviceProvider;
        }

        /// <summary>
        /// 创建并根据配置绑定指定类型对象的单例
        /// </summary>
        /// <typeparam name="TConfig">Configuration parameters</typeparam>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Set of key/value application configuration properties</param>
        /// <returns>Instance of configuration parameters</returns>
        public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            //create instance of config
            var config = new TConfig();

            //bind it to the appropriate section of configuration
            configuration.Bind(config);

            //and register it as a service
            services.AddSingleton(config);

            return config;
        }

        /// <summary>
        /// 注册 HttpContextAccessor 单例
        /// </summary>
        /// <param name="services"></param>
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        ///// <summary>
        ///// 添加  anti-forgery， 支持CSRF防范
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        //public static void AddAntiForgery(this IServiceCollection services)
        //{
        //    //override cookie name
        //    services.AddAntiforgery(options =>
        //    {
        //        options.Cookie.Name = "maple-csrf-c";
        //        options.FormFieldName = "maple-csrf-f";
        //        options.HeaderName = "maple-csrf-h";
        //    });
        //}

        ///// <summary>
        ///// 添加应用程序 session 支持（基于cookie 且仅http）
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        //public static void AddHttpSession(this IServiceCollection services)
        //{
        //    services.AddSession(options =>
        //    {
        //        options.Cookie.Name = "maple.session";
        //        options.Cookie.HttpOnly = true;
        //    });
        //}

        ///// <summary>
        ///// 添加皮肤的支持
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        //public static void AddThemes(this IServiceCollection services)
        //{
        //    if (!DataSettingsHelper.DatabaseIsInstalled())
        //        return;

        //    //themes support
        //    services.Configure<RazorViewEngineOptions>(options =>
        //    {
        //        options.ViewLocationExpanders.Add(new ThemeableViewLocationExpander());
        //    });
        //}

        ///// <summary>
        ///// 配置数据保护系统以将密钥保存的位置
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        //public static void AddMapleDataProtection(this IServiceCollection services)
        //{
        //    var dataProtectionKeysPath = CommonHelper.MapPath("~/App_Data/DataProtectionKeys");
        //    var dataProtectionKeysFolder = new DirectoryInfo(dataProtectionKeysPath);
        //    //配置数据保护系统以将密钥保存到指定的目录
        //    services.AddDataProtection().PersistKeysToFileSystem(dataProtectionKeysFolder);

        //    ////检查是否启用了Redis 并且 是否将数据保护组件中的私钥存储于Redis中
        //    //var mapleConfig = services.BuildServiceProvider().GetRequiredService<MapleConfig>();
        //    //if (mapleConfig.RedisCachingEnabled && mapleConfig.PersistDataProtectionKeysToRedis)
        //    //{
        //    //    services.AddDataProtection().PersistKeysToRedis(
        //    //        () =>
        //    //        {
        //    //            var redisConnectionWrapper = EngineContext.Current.Resolve<IRedisConnectionWrapper>();
        //    //            return redisConnectionWrapper.GetDatabase();
        //    //        }, RedisConfiguration.DataProtectionKeysName);
        //    //}
        //    //else
        //    //{
        //    //    var dataProtectionKeysPath = CommonHelper.MapPath("~/App_Data/DataProtectionKeys");
        //    //    var dataProtectionKeysFolder = new DirectoryInfo(dataProtectionKeysPath);

        //    //    //配置数据保护系统以将密钥保存到指定的目录
        //    //    services.AddDataProtection().PersistKeysToFileSystem(dataProtectionKeysFolder);
        //    //}
        //}

        ///// <summary>
        ///// 增加认证服务
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        //public static void AddMapleAuthentication(this IServiceCollection services)
        //{
        //    //设置默认身份验证方案
        //    var authenticationBuilder = services.AddAuthentication(options =>
        //    {
        //        options.DefaultChallengeScheme = NopCookieAuthenticationDefaults.AuthenticationScheme;
        //        options.DefaultSignInScheme = NopCookieAuthenticationDefaults.ExternalAuthenticationScheme;
        //    });

        //    //添加cookie身份验证
        //    authenticationBuilder.AddCookie(NopCookieAuthenticationDefaults.AuthenticationScheme, options =>
        //    {
        //        options.Cookie.Name = NopCookieAuthenticationDefaults.CookiePrefix + NopCookieAuthenticationDefaults.AuthenticationScheme;
        //        options.Cookie.HttpOnly = true;
        //        options.LoginPath = NopCookieAuthenticationDefaults.LoginPath;
        //        options.AccessDeniedPath = NopCookieAuthenticationDefaults.AccessDeniedPath;
        //    });

        //    //添加外部认证
        //    authenticationBuilder.AddCookie(NopCookieAuthenticationDefaults.ExternalAuthenticationScheme, options =>
        //    {
        //        options.Cookie.Name = NopCookieAuthenticationDefaults.CookiePrefix + NopCookieAuthenticationDefaults.ExternalAuthenticationScheme;
        //        options.Cookie.HttpOnly = true;
        //        options.LoginPath = NopCookieAuthenticationDefaults.LoginPath;
        //        options.AccessDeniedPath = NopCookieAuthenticationDefaults.AccessDeniedPath;
        //    });

        //    //注册和配置外部身份验证插件
        //    var typeFinder = new WebAppTypeFinder();
        //    var externalAuthConfigurations = typeFinder.FindClassesOfType<IExternalAuthenticationRegistrar>();
        //    var externalAuthInstances = externalAuthConfigurations
        //        .Where(x => PluginManager.FindPlugin(x)?.Installed ?? true) //ignore not installed plugins
        //        .Select(x => (IExternalAuthenticationRegistrar)Activator.CreateInstance(x));

        //    foreach (var instance in externalAuthInstances)
        //        instance.Configure(authenticationBuilder);
        //}

        /// <summary>
        /// 为应用程序添加和配置MVC
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddMapleMvc(this IServiceCollection services)
        {
            //add basic MVC feature
            var mvcBuilder = services.AddMvc();
            //use session temp data provider
            mvcBuilder.AddSessionStateTempDataProvider();
            //避免JSON序列化时大小写敏感的问题
            mvcBuilder.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            //添加自定义显示元数据提供程序
            mvcBuilder.AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new MapleMetadataProvider()));
            //添加自定义模型绑定器提供程序（到提供者列表的顶部）
            mvcBuilder.AddMvcOptions(options => options.ModelBinderProviders.Insert(0, new MapleModelBinderProvider()));

            ////加上 fluent 验证
            //mvcBuilder.AddFluentValidation(configuration => configuration.ValidatorFactoryType = typeof(NopValidatorFactory));

            return mvcBuilder;
        }
    }
}
