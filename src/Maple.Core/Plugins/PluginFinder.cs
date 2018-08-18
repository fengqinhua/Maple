using Maple.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maple.Core.Plugins
{
    /// <summary>
    /// Plugin finder
    /// </summary>
    public class PluginFinder : IPluginFinder
    {
        #region 属性字段

        private IList<PluginDescriptor> _plugins;
        private bool _arePluginsLoaded;

        #endregion

        #region 非共有方法

        /// <summary>
        /// 确保插件已经被加载
        /// </summary>
        protected virtual void EnsurePluginsAreLoaded()
        {
            if (!_arePluginsLoaded)
            {
                var foundPlugins = PluginManager.ReferencedPlugins.ToList();
                foundPlugins.Sort();
                _plugins = foundPlugins.ToList();

                _arePluginsLoaded = true;
            }
        }

        /// <summary>
        /// 检查插件的加载模式是否符合要求
        /// </summary>
        /// <param name="pluginDescriptor"></param>
        /// <param name="loadMode"></param>
        /// <returns></returns>
        protected virtual bool CheckLoadMode(PluginDescriptor pluginDescriptor, LoadPluginsMode loadMode)
        {
            Check.NotNull(pluginDescriptor, nameof(pluginDescriptor));

            switch (loadMode)
            {
                case LoadPluginsMode.All:
                    //no filering
                    return true;
                case LoadPluginsMode.InstalledOnly:
                    return pluginDescriptor.Installed;
                case LoadPluginsMode.NotInstalledOnly:
                    return !pluginDescriptor.Installed;
                default:
                    throw new MapleException("Not supported LoadPluginsMode");
            } 
        }

        /// <summary>
        /// 检查插件的分组是否符合要求
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="group">Group</param>
        /// <returns>true - available; false - no</returns>
        protected virtual bool CheckGroup(PluginDescriptor pluginDescriptor, string group)
        {
            Check.NotNull(pluginDescriptor, nameof(pluginDescriptor));

            if (string.IsNullOrEmpty(group))
                return true;

            return group.Equals(pluginDescriptor.Group, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region 共有方法

        /// <summary>
        /// 检查插件在某一个仓库中是否可用
        /// </summary>
        /// <param name="pluginDescriptor">插件的描述信息</param>
        /// <param name="storeId">仓库的标识符</param>
        /// <returns></returns>
        public virtual bool AuthenticateStore(PluginDescriptor pluginDescriptor, int storeId)
        {
            Check.NotNull(pluginDescriptor, nameof(pluginDescriptor));

            //no validation required
            if (storeId == 0)
                return true;

            if (!pluginDescriptor.LimitedToStores.Any())
                return true;

            return pluginDescriptor.LimitedToStores.Contains(storeId);
        }

        /// <summary>
        /// 检查插件是否能够授权给某一个用户使用
        /// </summary>
        /// <param name="pluginDescriptor"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public virtual bool AuthorizedForUser(PluginDescriptor pluginDescriptor, Customer customer)
        {
            Check.NotNull(pluginDescriptor, nameof(pluginDescriptor));

            if (customer == null || !pluginDescriptor.LimitedToCustomerRoles.Any())
                return true;

            //var customerRoleIds = customer.CustomerRoles.Where(role => role.Active).Select(role => role.Id);

            //return pluginDescriptor.LimitedToCustomerRoles.Intersect(customerRoleIds).Any();

            return true;
        }

        /// <summary>
        /// 获取插件分组清单
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetPluginGroups()
        {
            return GetPluginDescriptors(LoadPluginsMode.All).Select(x => x.Group).Distinct().OrderBy(x => x);
        }

        /// <summary>
        /// 获取现有的插件集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loadMode">插件加载模式（默认仅获取已加载的）</param>
        /// <param name="customer">用户（如果未设置，则加载哪些插件不受用户的限制）</param>
        /// <param name="storeId">仓储ID（如果传入0，则加载哪些插件不受仓库的限制）</param>
        /// <param name="group">插件分组（如果传入NULL，则加载哪些插件不受分组的限制</param>
        /// <returns>Plugins</returns>
        public virtual IEnumerable<T> GetPlugins<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null) where T : class, IPlugin
        {
            return GetPluginDescriptors<T>(loadMode, customer, storeId, group).Select(p => p.Instance<T>());
        }

        /// <summary>
        /// 获取现有的插件描述信息集合
        /// </summary>
        /// <param name="loadMode">插件加载模式（默认仅获取已加载的）</param>
        /// <param name="customer">用户（如果未设置，则加载哪些插件不受用户的限制）</param>
        /// <param name="storeId">仓储ID（如果传入0，则加载哪些插件不受仓库的限制）</param>
        /// <param name="group">插件分组（如果传入NULL，则加载哪些插件不受分组的限制</param>
        /// <returns> </returns>
        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null)
        {
            //确保插件均已被加载
            EnsurePluginsAreLoaded();

            return _plugins.Where(p => CheckLoadMode(p, loadMode) && AuthorizedForUser(p, customer) && AuthenticateStore(p, storeId) && CheckGroup(p, group));
        }

        /// <summary>
        /// 获取现有的插件描述信息集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loadMode">插件加载模式（默认仅获取已加载的）</param>
        /// <param name="customer">用户（如果未设置，则加载哪些插件不受用户的限制）</param>
        /// <param name="storeId">仓储ID（如果传入0，则加载哪些插件不受仓库的限制）</param>
        /// <param name="group">插件分组（如果传入NULL，则加载哪些插件不受分组的限制</param>
        /// <returns></returns>
        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null)
            where T : class, IPlugin
        {
            return GetPluginDescriptors(loadMode, customer, storeId, group)
                .Where(p => typeof(T).IsAssignableFrom(p.PluginType));
        }

        /// <summary>
        /// 获取某一指定插件的描述信息
        /// </summary>
        /// <param name="systemName">插件名称</param>
        /// <param name="loadMode">插件加载模式（默认仅获取已加载的）</param>
        /// <returns></returns>
        public virtual PluginDescriptor GetPluginDescriptorBySystemName(string systemName, LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly)
        {
            return GetPluginDescriptors(loadMode)
                .SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// 获取某一指定插件的描述信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="systemName">插件名称</param>
        /// <param name="loadMode">插件加载模式（默认仅获取已加载的）</param>
        /// <returns>></returns>
        public virtual PluginDescriptor GetPluginDescriptorBySystemName<T>(string systemName, LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly)
            where T : class, IPlugin
        {
            return GetPluginDescriptors<T>(loadMode)
                .SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public virtual void ReloadPlugins()
        {
            _arePluginsLoaded = false;
            EnsurePluginsAreLoaded();
        }

        #endregion
    }
}
