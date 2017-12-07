namespace Maple.Core.Plugins
{
    /// <summary>
    /// 接口：插件
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 获取配置页面的URL
        /// </summary>
        string GetConfigurationPageUrl();

        /// <summary>
        /// 获取插件的描述信息
        /// </summary>
        PluginDescriptor PluginDescriptor { get; set; }

        /// <summary>
        /// 安装插件
        /// </summary>
        void Install();

        /// <summary>
        /// 卸载插件
        /// </summary>
        void Uninstall();
    }
}
