using System;
using System.Collections.Generic;
using System.IO;
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
            if (string.IsNullOrWhiteSpace(this.WebHostPath) || !Directory.Exists(this.WebHostPath))
            {
                Log.LogWarning($"WebHostPath ({0}) is IsNullOrWhiteSpace or Directory is not exist !", this.WebHostPath);
                return true;
            }

            if (string.IsNullOrWhiteSpace(this.PluginPath) || !Directory.Exists(this.PluginPath))
            {
                Log.LogWarning($"PluginPath ({0}) is IsNullOrWhiteSpace or Directory is not exist !", this.PluginPath);
                return true;
            }

            string[] dllFileNames = getDllFileNames(this.WebHostPath);
            string[] filesToDeletes = new string[]
            {
                "dotnet-bundle.exe",
                "Maple.Web.App.pdb",
                "Maple.Web.App.exe",
                "Maple.Web.App.exe.config",
                "Maple.Web.Framework.pdb",
                "Maple.Web.Framework.dll",
                "Maple.Services.pdb",
                "Maple.Services.dll",
                "Maple.Core.pdb",
                "Maple.Core.dll"
            };

            try
            {
                var pluginDirectoryInfo = new DirectoryInfo(this.PluginPath);
                var allDirectoryInfo = new List<DirectoryInfo> { pluginDirectoryInfo };

                if (!SaveLocalesFolders)
                    allDirectoryInfo.AddRange(pluginDirectoryInfo.GetDirectories());

                foreach (var directoryInfo in allDirectoryInfo)
                {
                    foreach (var fileName in dllFileNames)
                    {
                        var dllfilePath = Path.Combine(directoryInfo.FullName, fileName + ".dll");
                        if (File.Exists(dllfilePath))
                            File.Delete(dllfilePath);
                        var pdbfilePath = Path.Combine(directoryInfo.FullName, fileName + ".pdb");
                        if (File.Exists(pdbfilePath))
                            File.Delete(pdbfilePath);
                    }

                    foreach (var fileName in filesToDeletes)
                    {
                        //delete file if it exist in current path
                        var pdbfilePath = Path.Combine(directoryInfo.FullName, fileName);
                        if (File.Exists(pdbfilePath))
                            File.Delete(pdbfilePath);
                    }

                    if (!directoryInfo.GetFiles().Any() && !directoryInfo.GetDirectories().Any() && !SaveLocalesFolders)
                        directoryInfo.Delete(true);

                }
            }
            catch (Exception ex)
            {
                Log.LogError($"has error.Message : {0} \n StackTrace : {1}", ex.Message, ex.StackTrace);
            }



            return true;
        }

        /// <summary>
        /// 获取 WebHost 项目bin所在目录下的所有dll文件名称（含子目录）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string[] getDllFileNames(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
                return Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories).Distinct().ToArray();
            else
                return new string[] { };
        }

    }
}
