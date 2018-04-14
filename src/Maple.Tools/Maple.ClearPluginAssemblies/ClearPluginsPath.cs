using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Maple.ClearPluginAssemblies
{
    public class ClearPluginsPath : Microsoft.Build.Utilities.Task
    {
        private string outputFile;

        [Microsoft.Build.Framework.Required]
        public string OutputFile
        {
            get { return outputFile; }
            set { outputFile = value; }
        }

        public override bool Execute()
        {
            Log.LogWarning("test message:" + this.outputFile);
            return true;
        }


        ///// <summary>
        ///// WebHost 项目bin所在目录
        ///// </summary>
        //[Microsoft.Build.Framework.Required]
        //public string WebHostPath { get; set; }

        ///// <summary>
        ///// 当前项目所在目录
        ///// </summary>
        //[Microsoft.Build.Framework.Required]
        //public string PluginPath { get; set; }

        ///// <summary>
        ///// 是否保留当前项目的多语言设置文件夹
        ///// </summary>
        //[Microsoft.Build.Framework.Required]
        //public bool SaveLocalesFolders { get; set; }

        ////        <!-- 所有插件生成的目录（多个用;隔开） -->
        ////< ParameterType = "System.String" />
        ////< !--所有需要删除的文件名称集合（多个用;隔开） -->
        ////< ParameterType = "System.String" />
        ////< !--当前项目（插件）的输出目录。如果该值不为空，则将使用该值替换Paths -->
        ////< ParameterType = "System.String" />
        ////< !---- >
        ////<  ParameterType="System.Boolean" />


        //public override bool Execute()
        //{
        //    Log.LogWarning("WebHost 项目bin所在目录:" + WebHostPath);
        //    Log.LogWarning("当前项目所在目录:" + PluginPath);
        //    Log.LogWarning("是否保留当前项目的多语言设置文件夹:" + SaveLocalesFolders);



        //    return true;
        //}


    }
}
