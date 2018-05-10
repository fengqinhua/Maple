using Maple.Core;
using Maple.Core.Configuration;
using Maple.Core.Data;
using Maple.Core.Http;
using Maple.Core.Infrastructure;
using Maple.Web.Framework.Globalization;
using Maple.Web.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Maple.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// 扩展 IApplicationBuilder 的方法
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);
        }


        /// <summary>
        /// MVC路由配置
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseMapleMvc(this IApplicationBuilder application)
        {
            application.UseMvc(routeBuilder =>
            {
                //register all routes
                EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(routeBuilder);
            });
        }

        /// <summary>
        /// 添加异常处理设置
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseMapleExceptionHandler(this IApplicationBuilder application)
        {
            var mapleConfig = EngineContext.Current.Resolve<MapleConfig>();
            var hostingEnvironment = EngineContext.Current.Resolve<IHostingEnvironment>();
            var useDetailedExceptionPage = mapleConfig.DisplayFullErrorStack || hostingEnvironment.IsDevelopment();
            if (useDetailedExceptionPage)
            {
                //为开发和测试目的获得详细的异常
                application.UseDeveloperExceptionPage();
            }
            else
            {
                //为最终用户使用特殊异常处理程序
                application.UseExceptionHandler("/errorpage.htm");
            }

            //异常情况记录
            application.UseExceptionHandler(handler =>
            {
                handler.Run(context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception == null)
                        return Task.CompletedTask;
                    try
                    {
                        //检查数据库是否已经安装配置
                        if (DataSettingsHelper.DatabaseIsInstalled())
                        {
                            //???
                            ////get current customer
                            //var currentCustomer = EngineContext.Current.Resolve<IWorkContext>().CurrentCustomer;
                            ////log error
                            //EngineContext.Current.Resolve<ILogger>().Error(exception.Message, exception, currentCustomer);
                        }
                    }
                    finally
                    {
                        //rethrow the exception to show the error page
                        throw exception;
                    }
                });
            });
        }

        /// <summary>
        /// 添加400异常处理设置
        /// </summary>
        /// <param name="application"></param>
        public static void UseBadRequestResult(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(context =>
            {
                //handle 404 (Bad request)
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    //???
                    //var logger = EngineContext.Current.Resolve<ILogger>();
                    //var workContext = EngineContext.Current.Resolve<IWorkContext>();
                    //logger.Error("Error 400. Bad request", null, customer: workContext.CurrentCustomer);
                }

                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// 添加404异常处理设置
        /// </summary>
        /// <param name="application"></param>
        public static void UsePageNotFound(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                //handle 404 Not Found
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    if (!webHelper.IsStaticResource())
                    {
                        //获取地址和查询条件
                        var originalPath = context.HttpContext.Request.Path;
                        var originalQueryString = context.HttpContext.Request.QueryString;

                        //store the original paths in special feature, so we can use it later
                        context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature()
                        {
                            OriginalPathBase = context.HttpContext.Request.PathBase.Value,
                            OriginalPath = originalPath.Value,
                            OriginalQueryString = originalQueryString.HasValue ? originalQueryString.Value : null,
                        });

                        //get new path
                        context.HttpContext.Request.Path = "/page-not-found.htm";
                        context.HttpContext.Request.QueryString = QueryString.Empty;

                        try
                        {
                            //re-execute request with new path
                            await context.Next(context.HttpContext);
                        }
                        finally
                        {
                            //return original path to request
                            context.HttpContext.Request.QueryString = originalQueryString;
                            context.HttpContext.Request.Path = originalPath;
                            context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(null);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 配置中间件检查请求的页面是否保持活动页面
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseKeepAlive(this IApplicationBuilder application)
        {
            application.UseMiddleware<KeepAliveMiddleware>();
        }

        /// <summary>
        /// 配置中间件 检测数据是否已经完成配置
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseInstallUrl(this IApplicationBuilder application)
        {
            application.UseMiddleware<InstallUrlMiddleware>();
        }

        /// <summary>
        /// 添加身份验证中间件
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseMapleAuthentication(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            application.UseMiddleware<Services.Authentication.AuthenticationMiddleware>();
        }

        /// <summary>
        /// 配置处理多语言的中间件
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseCulture(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;
            application.UseMiddleware<CultureMiddleware>();
        }


    }
}
