using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Maple.Core.Infrastructure
{
    /// <summary>
    /// 提供当前Web应用程序中类型的信息。可选地，这个类可以查看bin文件夹中的所有程序集。
    /// </summary>
    public class WebAppTypeFinder : AppDomainTypeFinder
    {
        #region 属性字段

        private bool _ensureBinFolderAssembliesLoaded = true;
        private bool _binFolderAssembliesLoaded;

        #endregion

        #region Properties

        /// <summary>
        /// 是否获取Bin目录下的程序集信息
        /// </summary>
        public bool EnsureBinFolderAssembliesLoaded
        {
            get { return _ensureBinFolderAssembliesLoaded; }
            set { _ensureBinFolderAssembliesLoaded = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 获取Web应用程序bin文件夹的物理目录
        /// </summary>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public virtual string GetBinDirectory()
        {
            return System.AppContext.BaseDirectory;
        }

        /// <summary>
        /// 获取程序集信息
        /// </summary>
        /// <returns>Result</returns>
        public override IList<Assembly> GetAssemblies()
        {
            if (this.EnsureBinFolderAssembliesLoaded && !_binFolderAssembliesLoaded)
            {
                _binFolderAssembliesLoaded = true;
                var binPath = GetBinDirectory();
                //binPath = _webHelper.MapPath("~/bin");
                LoadMatchingAssemblies(binPath);
            }

            return base.GetAssemblies();
        }

        #endregion
    }
}
