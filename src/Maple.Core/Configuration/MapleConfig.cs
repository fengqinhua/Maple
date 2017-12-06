using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Configuration
{
    /// <summary>
    /// 定义 startup 配置参数
    /// </summary>
    public partial class MapleConfig
    {
        /// <summary>
        /// 获取或设置值，是否在生产环境中显示完整错误。
        /// 在开发环境中被忽略（总是启用）
        /// </summary>
        public bool DisplayFullErrorStack { get; set; }

        /// <summary>
        /// 获取或设置请求和响应静态文件时遵循的缓存机制
        /// </summary>
        public string StaticFilesCacheControl { get; set; }

        /// <summary>
        /// 获取或设置值，指示是否压缩响应
        /// </summary>
        public bool UseResponseCompression { get; set; }

        /// <summary>
        /// 获取或设置用于Azure存储的连接字符串
        /// </summary>
        public string AzureBlobStorageConnectionString { get; set; }
        /// <summary>
        /// 获取或设置用于Azure存储的容器名称
        /// </summary>
        public string AzureBlobStorageContainerName { get; set; }
        /// <summary>
        /// 获取或设置用于Azure存储的 end point
        /// </summary>
        public string AzureBlobStorageEndPoint { get; set; }

        /// <summary>
        /// 获取或设置 是否启用Redis进行数据缓存，启用后将替代缺省的基于内存的缓存管理器
        /// </summary>
        public bool RedisCachingEnabled { get; set; }
        /// <summary>
        /// 获取或设置 Redis 链接字符串
        /// </summary>
        public string RedisCachingConnectionString { get; set; }
        /// <summary>
        /// 获取或设置 是否将数据保护组件中的私钥存储于Redis中
        /// </summary>
        public bool PersistDataProtectionKeysToRedis { get; set; }

        /// <summary>
        /// 获取或设置 配置客户端用户信息数据库的路径
        /// </summary>
        public string UserAgentStringsPath { get; set; }
        /// <summary>
        /// Gets or sets path to database with crawler only user agent strings
        /// </summary>
        public string CrawlerOnlyUserAgentStringsPath { get; set; }

        /// <summary>
        /// 获取或设置 是否支持Maple老版本的组件
        /// </summary>
        public bool SupportPreviousMapleVersions { get; set; }

        /// <summary>
        /// 获取或设置 在安装过程中是否初始化示例数据
        /// </summary>
        public bool DisableSampleDataDuringInstallation { get; set; }
        /// <summary>
        /// 获取或设置 是否支持快速安装. 
        /// By default this setting should always be set to "False" (only for advanced users)
        /// </summary>
        public bool UseFastInstallationService { get; set; }
        /// <summary>
        /// 获取或设置 一个忽略安装的插件列表，多个插件之间使用,分割
        /// </summary>
        public string PluginsIgnoredDuringInstallation { get; set; }

        /// <summary>
        /// 获取或设置 是否忽略启动任务
        /// </summary>
        public bool IgnoreStartupTasks { get; set; }

        /// <summary>
        /// 获取或设置 程序启动时是否清理 /Plugins/bin 目录
        /// </summary>
        public bool ClearPluginShadowDirectoryOnStartup { get; set; }

        /// <summary>
        /// 获取或设置 是否绕过一些安全检查直接将插件加载至应用程序上下文中
        /// </summary>
        public bool UseUnsafeLoadAssembly { get; set; }
    }
}
