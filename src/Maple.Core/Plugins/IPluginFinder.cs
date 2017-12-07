using System.Collections.Generic;
using Maple.Core.Domain.Customers;

namespace Maple.Core.Plugins
{
    /// <summary>
    /// 接口：插件的发现者
    /// </summary>
    public interface IPluginFinder
    {
        /// <summary>
        /// 检查插件在某一个仓库中是否可用
        /// </summary>
        /// <param name="pluginDescriptor">插件的描述信息</param>
        /// <param name="storeId">仓库的标识符</param>
        /// <returns></returns>
        bool AuthenticateStore(PluginDescriptor pluginDescriptor, int storeId);

        /// <summary>
        /// 检查插件是否能够授权给某一个用户使用
        /// </summary>
        /// <param name="pluginDescriptor">插件的描述信息/param>
        /// <param name="customer">用户信息</param>
        /// <returns></returns>
        bool AuthorizedForUser(PluginDescriptor pluginDescriptor, Customer customer);

        /// <summary>
        /// 获取插件分组清单
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetPluginGroups();

        /// <summary>
        /// 获取现有的插件集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loadMode">插件加载模式（默认仅获取已加载的）</param>
        /// <param name="customer">用户（如果未设置，则加载哪些插件不受用户的限制）</param>
        /// <param name="storeId">仓储ID（如果传入0，则加载哪些插件不受仓库的限制）</param>
        /// <param name="group">插件分组（如果传入NULL，则加载哪些插件不受分组的限制</param>
        /// <returns>Plugins</returns>
        IEnumerable<T> GetPlugins<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null) where T : class, IPlugin;

        /// <summary>
        /// 获取现有的插件描述信息集合
        /// </summary>
        /// <param name="loadMode">插件加载模式（默认仅获取已加载的）</param>
        /// <param name="customer">用户（如果未设置，则加载哪些插件不受用户的限制）</param>
        /// <param name="storeId">仓储ID（如果传入0，则加载哪些插件不受仓库的限制）</param>
        /// <param name="group">插件分组（如果传入NULL，则加载哪些插件不受分组的限制</param>
        /// <returns> </returns>
        IEnumerable<PluginDescriptor> GetPluginDescriptors(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null);

        /// <summary>
        /// 获取现有的插件描述信息集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loadMode">插件加载模式（默认仅获取已加载的）</param>
        /// <param name="customer">用户（如果未设置，则加载哪些插件不受用户的限制）</param>
        /// <param name="storeId">仓储ID（如果传入0，则加载哪些插件不受仓库的限制）</param>
        /// <param name="group">插件分组（如果传入NULL，则加载哪些插件不受分组的限制</param>
        /// <returns></returns>
        IEnumerable<PluginDescriptor> GetPluginDescriptors<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null) where T : class, IPlugin;

        /// <summary>
        /// 获取某一指定插件的描述信息
        /// </summary>
        /// <param name="systemName">插件名称</param>
        /// <param name="loadMode">插件加载模式（默认仅获取已加载的）</param>
        /// <returns></returns>
        PluginDescriptor GetPluginDescriptorBySystemName(string systemName, LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly);

        /// <summary>
        /// 获取某一指定插件的描述信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="systemName">插件名称</param>
        /// <param name="loadMode">插件加载模式（默认仅获取已加载的）</param>
        /// <returns>></returns>
        PluginDescriptor GetPluginDescriptorBySystemName<T>(string systemName, LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly)
            where T : class, IPlugin;

        /// <summary>
        /// 刷新
        /// </summary>
        void ReloadPlugins();
    }
}
