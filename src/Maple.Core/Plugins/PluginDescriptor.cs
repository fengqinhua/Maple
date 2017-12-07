using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Maple.Core.Infrastructure;

namespace Maple.Core.Plugins
{
    /// <summary>
    /// 插件描述信息的实现类
    /// </summary>
    public class PluginDescriptor : IDescriptor, IComparable<PluginDescriptor>
    {
        #region 构造函数

        /// <summary>
        /// 插件描述信息的实现类
        /// </summary>
        public PluginDescriptor()
        {
            this.SupportedVersions = new List<string>();
            this.LimitedToStores = new List<int>();
            this.LimitedToCustomerRoles = new List<int>();
        }

        /// <summary>
        /// 插件描述信息的实现类
        /// </summary>
        /// <param name="referencedAssembly">Referenced assembly</param>
        public PluginDescriptor(Assembly referencedAssembly) : this()
        {
            this.ReferencedAssembly = referencedAssembly;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 获取插件实例
        /// </summary>
        /// <returns></returns>
        public IPlugin Instance()
        {
            return Instance<IPlugin>();
        }

        /// <summary>
        /// 获取插件实例
        /// </summary>
        /// <typeparam name="T">插件类型</typeparam>
        /// <returns></returns>
        public virtual T Instance<T>() where T : class, IPlugin
        {
            object instance = null;
            try
            {
                instance = EngineContext.Current.Resolve(PluginType);
            }
            catch
            {
                //try resolve
            }
            if (instance == null)
            {
                //not resolved
                instance = EngineContext.Current.ResolveUnregistered(PluginType);
            }
            var typedInstance = instance as T;
            if (typedInstance != null)
                typedInstance.PluginDescriptor = this;
            return typedInstance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PluginDescriptor other)
        {
            //首先比较DisplayOrder，如果一致则比较FriendlyName
            if (DisplayOrder != other.DisplayOrder)
                return DisplayOrder.CompareTo(other.DisplayOrder);

            return FriendlyName.CompareTo(other.FriendlyName);
        }

        /// <summary>
        /// 获取插件别名
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FriendlyName;
        }

        /// <summary>
        /// 比较SystemName
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool Equals(object value)
        {
            return SystemName?.Equals((value as PluginDescriptor)?.SystemName) ?? false;
        }

        /// <summary>
        /// 返回SystemName的HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return SystemName.GetHashCode();
        }

        #endregion

        #region 属性字段

        /// <summary>
        /// 分组
        /// </summary>
        [JsonProperty(PropertyName = "Group")]
        public virtual string Group { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [JsonProperty(PropertyName = "FriendlyName")]
        public virtual string FriendlyName { get; set; }

        /// <summary>
        /// 名称（唯一）
        /// </summary>
        [JsonProperty(PropertyName = "SystemName")]
        public virtual string SystemName { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [JsonProperty(PropertyName = "Version")]
        public virtual string Version { get; set; }

        /// <summary>
        /// 获取支持的Maple版本
        /// </summary>
        [JsonProperty(PropertyName = "SupportedVersions")]
        public virtual IList<string> SupportedVersions { get; set; }

        /// <summary>
        /// 获取作者信息
        /// </summary>
        [JsonProperty(PropertyName = "Author")]
        public virtual string Author { get; set; }

        /// <summary>
        /// 获取排列顺序
        /// </summary>
        [JsonProperty(PropertyName = "DisplayOrder")]
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        [JsonProperty(PropertyName = "FileName")]
        public virtual string AssemblyFileName { get; set; }

        /// <summary>
        /// 插件描述信息
        /// </summary>
        [JsonProperty(PropertyName = "Description")]
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the list of store identifiers in which this plugin is available. If empty, then this plugin is available in all stores
        /// </summary>
        [JsonProperty(PropertyName = "LimitedToStores")]
        public virtual IList<int> LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets the list of customer role identifiers for which this plugin is available. If empty, then this plugin is available for all ones.
        /// </summary>
        [JsonProperty(PropertyName = "LimitedToCustomerRoles")]
        public virtual IList<int> LimitedToCustomerRoles { get; set; }

        /// <summary>
        /// 插件是否被加载
        /// </summary>
        [JsonIgnore]
        public virtual bool Installed { get; set; }

        /// <summary>
        /// 插件类型
        /// </summary>
        [JsonIgnore]
        public virtual Type PluginType { get; set; }

        /// <summary>
        /// 插件对应的文件信息
        /// </summary>
        [JsonIgnore]
        public virtual FileInfo OriginalAssemblyFile { get; internal set; }

        /// <summary>
        /// 插件引用的程序集信息
        /// </summary>
        [JsonIgnore]
        public virtual Assembly ReferencedAssembly { get; internal set; }

        #endregion

    }
}