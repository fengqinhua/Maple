using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maple.BuildTask
{
    public class ClearPluginsPathTask : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// WebHost项目bin所在目录
        /// </summary>
        [Microsoft.Build.Framework.Required]
        public string WebHostPath { get; set; }
        /// <summary>
        /// 当前项目Build输出目录
        /// </summary>
        [Microsoft.Build.Framework.Required]
        public string PluginPath { get; set; }
        /// <summary>
        /// 是否保留当前项目的多语言设置文件夹
        /// </summary>
        [Microsoft.Build.Framework.Required]
        public bool SaveLocalesFolders { get; set; }


        public override bool Execute()
        {
            Log.LogWarning("WebHostPath = " + this.WebHostPath);
            Log.LogWarning("PluginPath = " + this.PluginPath);
            Log.LogWarning("SaveLocalesFolders = " + this.SaveLocalesFolders);
            return true;
        }
    }
}
