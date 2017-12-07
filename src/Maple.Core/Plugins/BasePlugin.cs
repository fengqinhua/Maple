using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Plugins
{
    /// <summary>
    /// 插件基类
    /// </summary>
    public abstract class BasePlugin : IPlugin
    {
        /// <summary>
        /// 获取配置页面的URL
        /// </summary>
        public virtual string GetConfigurationPageUrl()
        {
            return null;
        }

        /// <summary>
        /// 获取插件的描述信息
        /// </summary>
        public virtual PluginDescriptor PluginDescriptor { get; set; }

        /// <summary>
        /// 安装插件
        /// </summary>
        public virtual void Install()
        {
            PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);
        }

        /// <summary>
        /// 卸载插件
        /// </summary>
        public virtual void Uninstall()
        {
            PluginManager.MarkPluginAsUninstalled(this.PluginDescriptor.SystemName);
        }
    }
}
