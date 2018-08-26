using Maple.Core;
using Maple.Core.Configuration;
using Maple.Core.Data.DataSettings;
using Maple.Core.Infrastructure;
using Maple.Web.Framework.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Maple.Web.Framework.Infrastructure
{
    /// <summary>
    /// 用于处理应用程序启动时公用的配置和启用中间件
    /// </summary>
    public class MapleCommonStartup : IMapleStartup
    {

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {            
            //注册压缩中间件
            services.AddResponseCompression();
            //注册于针对Option模型的服务（用于DNC中配置信息下读取）
            services.AddOptions();
            //注册缓存服务（使用内存）
            services.AddMemoryCache();
            //注册分布式缓存服务
            services.AddDistributedMemoryCache();
            //注册Http Session服务
            services.AddHttpSession();
            //注册 anti-forgery服务
            services.AddAntiForgery();
            //注册多语言服务
            services.AddLocalization();
            //添加皮肤支持
            services.AddThemes();

            //不知道干什么用的
            ////add gif resizing support
            //new PrettyGifs().Install(Config.Current);
        }


        public void Configure(IApplicationBuilder application)
        {
            var mapleConfig = EngineContext.Current.Resolve<MapleConfig>();
            //启用压缩中间件
            if (mapleConfig.UseResponseCompression)
            {
                //gzip by default
                application.UseResponseCompression();

                //目前使用 DNC 2.0该问题已解决，故无需对此进行扩展了
                ////workaround with "vary" header
                //application.UseMiddleware<Compression.ResponseCompressionVaryWorkaroundMiddleware>();
            }
            //标识静态文件缓存
            application.UseStaticFiles(new StaticFileOptions
            {
                //TODO duplicated code (below)
                OnPrepareResponse = ctx =>
                {
                    if (!string.IsNullOrEmpty(mapleConfig.StaticFilesCacheControl))
                        ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, mapleConfig.StaticFilesCacheControl);
                }
            });
            //默认的，静态文件存储在你的webroot目录下面，webroot的路径定义在project.json里面 "webroot": "wwwroot" ，通过UseStaticFiles() 启用
            //如果有不在webroot目录下的静态文件需要添加，那么可以通过定义FileProvider 和RequestPath 来进行设置
            //如下设置完成后,访问 http://localhost/Themes/xxxfile 则是从 当前应用程序目录中的 Themes/xxxfile 查找
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Themes")),
                RequestPath = new PathString("/Themes"),
                OnPrepareResponse = ctx =>
                {
                    //标识静态文件缓存
                    if (!string.IsNullOrEmpty(mapleConfig.StaticFilesCacheControl))
                        ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, mapleConfig.StaticFilesCacheControl);
                }
            });
            //处理插件中的静态资源
            var staticFileOptions = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Plugins")),
                RequestPath = new PathString("/Plugins"),
                OnPrepareResponse = ctx =>
                {
                    //标识静态文件缓存
                    if (!string.IsNullOrEmpty(mapleConfig.StaticFilesCacheControl))
                        ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, mapleConfig.StaticFilesCacheControl);
                }
            };
            if (MainDataSettingsHelper.DatabaseIsInstalled())
            {
                //此处重写，将该配置直接写入MapleConfig中
                if (!string.IsNullOrEmpty(mapleConfig.PluginStaticFileExtensionsBlacklist))
                {
                    //此类包含一个将文件扩展名映射到MIME内容类型的集合
                    //将文件后缀从列表中移除可达到设置黑名单静态文件的目的

                    var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
                    foreach (var ext in mapleConfig.PluginStaticFileExtensionsBlacklist
                                                   .Split(';', ',')
                                                   .Select(e => e.Trim().ToLower())
                                                   .Select(e => $"{(e.StartsWith(".") ? string.Empty : ".")}{e}")
                                                   .Where(fileExtensionContentTypeProvider.Mappings.ContainsKey))
                    {
                        fileExtensionContentTypeProvider.Mappings.Remove(ext);
                    }
                    staticFileOptions.ContentTypeProvider = fileExtensionContentTypeProvider;
                }

                //var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();
                //if (!string.IsNullOrEmpty(securitySettings.PluginStaticFileExtensionsBlacklist))
                //{
                //    var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
                //    foreach (var ext in securitySettings.PluginStaticFileExtensionsBlacklist
                //                                        .Split(';', ',')
                //                                        .Select(e => e.Trim().ToLower())
                //                                        .Select(e => $"{(e.StartsWith(".") ? string.Empty : ".")}{e}")
                //                                        .Where(fileExtensionContentTypeProvider.Mappings.ContainsKey))
                //    {
                //        fileExtensionContentTypeProvider.Mappings.Remove(ext);
                //    }
                //    staticFileOptions.ContentTypeProvider = fileExtensionContentTypeProvider;
                //} 
            }
            application.UseStaticFiles(staticFileOptions);
            //添加.bak文件下载支持
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".bak"] = MimeTypes.ApplicationOctetStream;
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "db_backups")),
                RequestPath = new PathString("/db_backups"),
                ContentTypeProvider = provider
            });
            //暂时还不知道有什么用途，检测网站是否处于运行状态？
            //check whether requested page is keep alive page
            application.UseKeepAlive();
            //启用Session
            application.UseSession();
            //启用多语言
            application.UseRequestLocalization();
        }

        public int Order
        {
            //异常处理的设置应该最先加载
            get { return 100; }
        }
    }
}
