using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Plugins
{
    /// <summary>
    /// 插件的加载模式
    /// </summary>
    public enum LoadPluginsMode
    {
        /// <summary>
        /// 加载所有插件 (已安装的或没有安装的)
        /// </summary>
        All = 0,
        /// <summary>
        /// 仅已安装的
        /// </summary>
        InstalledOnly = 10,
        /// <summary>
        /// 仅未安装的
        /// </summary>
        NotInstalledOnly = 20
    }
}
